using System;
using UnityEngine;
using UnityEngine.UI;

namespace IrakliChkuaseli.UI.UICorner
{
    public enum CornerStyle
    {
        Rounded = 0,
        Chamfered = 1
    }

    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Effects/UI Corner")]
    public class UICorner : MonoBehaviour
    {
        private static readonly int prop_halfSize = Shader.PropertyToID("_halfSize");
        private static readonly int prop_radiuses = Shader.PropertyToID("_r");
        private static readonly int prop_rect2props = Shader.PropertyToID("_rect2props");
        private static readonly int prop_cornerTypes = Shader.PropertyToID("_cornerTypes");
        private static readonly int prop_OuterUV = Shader.PropertyToID("_OuterUV");

        // Vector2.right rotated clockwise by 45 degrees
        private static readonly Vector2 wNorm = new(.7071068f, -.7071068f);

        // Vector2.right rotated counter-clockwise by 45 degrees
        private static readonly Vector2 hNorm = new(.7071068f, .7071068f);

        [Header("Corner Sizes")] public Vector4 r = new(40f, 40f, 40f, 40f);

        [Header("Corner Styles")] public CornerStyle topLeft = CornerStyle.Rounded;
        public CornerStyle topRight = CornerStyle.Rounded;
        public CornerStyle bottomRight = CornerStyle.Rounded;
        public CornerStyle bottomLeft = CornerStyle.Rounded;

        private Material material;
        private Vector4 outerUV = new(0, 0, 1, 1);

        // xy - position,
        // zw - halfSize
        [HideInInspector] [SerializeField] private Vector4 rect2props;
        [HideInInspector] [SerializeField] private MaskableGraphic image;

        /// <summary>
        /// Invoked when corner properties change (radius, style).
        /// Use for integrations like True Shadow, DOTween, etc.
        /// </summary>
        public event Action PropertiesChanged;

        private void OnValidate()
        {
            Validate();
            Refresh();
            PropertiesChanged?.Invoke();
        }

        private void OnEnable()
        {
            Validate();
            Refresh();
        }

        private void OnRectTransformDimensionsChange()
        {
            if (enabled && material != null) Refresh();
        }

        private void OnDestroy()
        {
            if (image != null) image.material = null;

            if (material != null) DestroyImmediate(material);
            image = null;
            material = null;
        }

        public void Validate()
        {
            if (material == null) material = new Material(Shader.Find("UI/UICorner"));

            if (image == null) TryGetComponent(out image);

            if (image != null) image.material = material;

            if (image is Image uiImage && uiImage.sprite != null)
                outerUV = UnityEngine.Sprites.DataUtility.GetOuterUV(uiImage.sprite);
        }

        public void Refresh()
        {
            if (material == null) return;

            var rect = ((RectTransform)transform).rect;

            // Clamp corner sizes to prevent overlap
            // Adjacent corners cannot sum to more than the edge length
            var clampedR = r;

            // Apply a safety factor to prevent geometry breakdown
            var safetyFactor = 0.99f;

            // Top edge: topLeft + topRight must be < width
            var topSum = clampedR.x + clampedR.y;
            if (topSum > rect.width * safetyFactor)
            {
                var scale = rect.width * safetyFactor / topSum;
                clampedR.x *= scale;
                clampedR.y *= scale;
            }

            // Bottom edge: bottomLeft + bottomRight must be < width
            var bottomSum = clampedR.w + clampedR.z;
            if (bottomSum > rect.width * safetyFactor)
            {
                var scale = rect.width * safetyFactor / bottomSum;
                clampedR.w *= scale;
                clampedR.z *= scale;
            }

            // Left edge: topLeft + bottomLeft must be < height
            var leftSum = clampedR.x + clampedR.w;
            if (leftSum > rect.height * safetyFactor)
            {
                var scale = rect.height * safetyFactor / leftSum;
                clampedR.x *= scale;
                clampedR.w *= scale;
            }

            // Right edge: topRight + bottomRight must be < height
            var rightSum = clampedR.y + clampedR.z;
            if (rightSum > rect.height * safetyFactor)
            {
                var scale = rect.height * safetyFactor / rightSum;
                clampedR.y *= scale;
                clampedR.z *= scale;
            }

            RecalculateProps(rect.size, clampedR);

            material.SetVector(prop_rect2props, rect2props);
            material.SetVector(prop_halfSize, rect.size * .5f);
            material.SetVector(prop_radiuses, clampedR);
            material.SetVector(prop_OuterUV, outerUV);

            // Set corner styles (0 = rounded, 1 = chamfered)
            var cornerStyles = new Vector4(
                (float)topLeft,
                (float)topRight,
                (float)bottomRight,
                (float)bottomLeft
            );
            material.SetVector(prop_cornerTypes, cornerStyles);
        }

        private void RecalculateProps(Vector2 size, Vector4 cornerRadii)
        {
            // Vector that goes from left to right sides of rect2
            var aVec = new Vector2(size.x, -size.y + cornerRadii.x + cornerRadii.z);

            // Project vector aVec to wNorm to get magnitude of rect2 width vector
            var halfWidth = Vector2.Dot(aVec, wNorm) * .5f;
            rect2props.z = halfWidth;

            // Vector that goes from bottom to top sides of rect2
            var bVec = new Vector2(size.x, size.y - cornerRadii.w - cornerRadii.y);

            // Project vector bVec to hNorm to get magnitude of rect2 height vector
            var halfHeight = Vector2.Dot(bVec, hNorm) * .5f;
            rect2props.w = halfHeight;

            // Vector that goes from left to top sides of rect2
            var efVec = new Vector2(size.x - cornerRadii.x - cornerRadii.y, 0);

            // Vector that goes from point E to point G, which is top-left of rect2
            var egVec = hNorm * Vector2.Dot(efVec, hNorm);

            // Position of point E relative to center of coord system
            var ePoint = new Vector2(cornerRadii.x - size.x / 2, size.y / 2);

            // Origin of rect2 relative to center of coord system
            // ePoint + egVec == vector to top-left corner of rect2
            // wNorm * halfWidth + hNorm * -halfHeight == vector from top-left corner to center
            var origin = ePoint + egVec + wNorm * halfWidth + hNorm * -halfHeight;
            rect2props.x = origin.x;
            rect2props.y = origin.y;
        }
    }
}