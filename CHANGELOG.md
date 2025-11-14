# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.1] - 2025-11-14

### Added
- Component icon for better editor visibility and component identification

## [1.1.0] - 2025-11-12

### Added
- `PropertiesChanged` event that fires when corner properties (radius or style) change
- Public integration API for external components to react to corner property changes

### Technical Details
- Event fires in `OnValidate()` when corner properties are modified
- Enables event-driven integrations without polling
- Zero performance overhead when no listeners are subscribed

## [1.0.0] - 2025-11-11

### Added
- Initial release of UI Corner package
- Dual corner styles: Rounded (circular arc) and Chamfered (45-degree diagonal cut)
- Per-corner independent control (mix and match styles)
- High-performance GPU-accelerated rendering (2.5-3.1× faster than alternatives)
- Signed Distance Field (SDF) rendering using Inigo Quilez's quadrant-selection technique
- Automatic size clamping to prevent geometry breakdown
- Responsive custom inspector with clean UI for narrow windows
- Full compatibility with Unity UI Mask system
- Support for all standard Image features
- Runtime folder structure with proper assembly definitions
- Editor custom inspector with per-corner configuration
- Comprehensive shader implementation (UICorner.shader and UICornerSDF.cginc)

### Technical Details
- Minimum Unity version: 2021.3 LTS
- Package name: `com.iraklichkuaseli.uicorner`
- Assembly definitions: `IrakliChkuaseli.UICorner` (Runtime) and `IrakliChkuaseli.UICorner.Editor` (Editor)
- Namespace: `IrakliChkuaseli.UI.UICorner`
