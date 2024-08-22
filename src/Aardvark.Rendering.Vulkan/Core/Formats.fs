﻿namespace Aardvark.Rendering.Vulkan

open System
open Aardvark.Base
open Aardvark.Rendering
open TypeMeta
open GLSLType.Interop.Patterns

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module VkFormat =
    let ofTextureFormat =
        LookupTable.lookup [
            TextureFormat.Bgr8,                           VkFormat.B8g8r8Unorm
            TextureFormat.Bgra8,                          VkFormat.B8g8r8a8Unorm
            TextureFormat.R3G3B2,                         VkFormat.Undefined
            TextureFormat.Rgb4,                           VkFormat.R4g4b4a4UnormPack16
            TextureFormat.Rgb5,                           VkFormat.R5g5b5a1UnormPack16
            TextureFormat.Rgb8,                           VkFormat.R8g8b8Unorm
            TextureFormat.Rgb10,                          VkFormat.A2b10g10r10UnormPack32
            TextureFormat.Rgb12,                          VkFormat.Undefined
            TextureFormat.Rgb16,                          VkFormat.R16g16b16Unorm
            TextureFormat.Rgba2,                          VkFormat.Undefined
            TextureFormat.Rgba4,                          VkFormat.R4g4b4a4UnormPack16
            TextureFormat.Rgb5A1,                         VkFormat.R5g5b5a1UnormPack16
            TextureFormat.Rgba8,                          VkFormat.R8g8b8a8Unorm
            TextureFormat.Rgb10A2,                        VkFormat.A2b10g10r10UnormPack32
            TextureFormat.Rgba12,                         VkFormat.Undefined
            TextureFormat.Rgba16,                         VkFormat.R16g16b16a16Unorm
            TextureFormat.R8,                             VkFormat.R8Unorm
            TextureFormat.R16,                            VkFormat.R16Unorm
            TextureFormat.Rg8,                            VkFormat.R8g8Unorm
            TextureFormat.Rg16,                           VkFormat.R16g16Unorm
            TextureFormat.R16f,                           VkFormat.R16Sfloat
            TextureFormat.R32f,                           VkFormat.R32Sfloat
            TextureFormat.Rg16f,                          VkFormat.R16g16Sfloat
            TextureFormat.Rg32f,                          VkFormat.R32g32Sfloat
            TextureFormat.R8i,                            VkFormat.R8Sint
            TextureFormat.R8ui,                           VkFormat.R8Uint
            TextureFormat.R16i,                           VkFormat.R16Sint
            TextureFormat.R16ui,                          VkFormat.R16Uint
            TextureFormat.R32i,                           VkFormat.R32Sint
            TextureFormat.R32ui,                          VkFormat.R32Uint
            TextureFormat.Rg8i,                           VkFormat.R8g8Sint
            TextureFormat.Rg8ui,                          VkFormat.R8g8Uint
            TextureFormat.Rg16i,                          VkFormat.R16g16Sint
            TextureFormat.Rg16ui,                         VkFormat.R16g16Uint
            TextureFormat.Rg32i,                          VkFormat.R32g32Sint
            TextureFormat.Rg32ui,                         VkFormat.R32g32Uint
            TextureFormat.Rgba32f,                        VkFormat.R32g32b32a32Sfloat
            TextureFormat.Rgb32f,                         VkFormat.R32g32b32Sfloat
            TextureFormat.Rgba16f,                        VkFormat.R16g16b16a16Sfloat
            TextureFormat.Rgb16f,                         VkFormat.R16g16b16Sfloat
            TextureFormat.R11fG11fB10f,                   VkFormat.B10g11r11UfloatPack32
            TextureFormat.Rgb9E5,                         VkFormat.E5b9g9r9UfloatPack32
            TextureFormat.Srgb8,                          VkFormat.R8g8b8Srgb
            TextureFormat.Srgb8Alpha8,                    VkFormat.R8g8b8a8Srgb
            TextureFormat.Rgba32ui,                       VkFormat.R32g32b32a32Uint
            TextureFormat.Rgb32ui,                        VkFormat.R32g32b32Uint
            TextureFormat.Rgba16ui,                       VkFormat.R16g16b16a16Uint
            TextureFormat.Rgb16ui,                        VkFormat.R16g16b16Uint
            TextureFormat.Rgba8ui,                        VkFormat.R8g8b8a8Uint
            TextureFormat.Rgb8ui,                         VkFormat.R8g8b8Uint
            TextureFormat.Rgba32i,                        VkFormat.R32g32b32a32Sint
            TextureFormat.Rgb32i,                         VkFormat.R32g32b32Sint
            TextureFormat.Rgba16i,                        VkFormat.R16g16b16a16Sint
            TextureFormat.Rgb16i,                         VkFormat.R16g16b16Sint
            TextureFormat.Rgba8i,                         VkFormat.R8g8b8a8Sint
            TextureFormat.Rgb8i,                          VkFormat.R8g8b8Sint
            TextureFormat.R8Snorm,                        VkFormat.R8Snorm
            TextureFormat.Rg8Snorm,                       VkFormat.R8g8Snorm
            TextureFormat.Rgb8Snorm,                      VkFormat.R8g8b8Snorm
            TextureFormat.Rgba8Snorm,                     VkFormat.R8g8b8a8Snorm
            TextureFormat.R16Snorm,                       VkFormat.R16Snorm
            TextureFormat.Rg16Snorm,                      VkFormat.R16g16Snorm
            TextureFormat.Rgb16Snorm,                     VkFormat.R16g16b16Snorm
            TextureFormat.Rgba16Snorm,                    VkFormat.R16g16b16a16Snorm
            TextureFormat.Rgb10A2ui,                      VkFormat.A2b10g10r10UintPack32
            TextureFormat.DepthComponent16,               VkFormat.D16Unorm
            TextureFormat.DepthComponent24,               VkFormat.X8D24UnormPack32
            TextureFormat.DepthComponent32,               VkFormat.D32Sfloat
            TextureFormat.DepthComponent32f,              VkFormat.D32Sfloat
            TextureFormat.Depth24Stencil8,                VkFormat.D24UnormS8Uint
            TextureFormat.Depth32fStencil8,               VkFormat.D32SfloatS8Uint
            TextureFormat.StencilIndex8,                  VkFormat.S8Uint

            TextureFormat.CompressedRgbS3tcDxt1,          VkFormat.Bc1RgbUnormBlock
            TextureFormat.CompressedSrgbS3tcDxt1,         VkFormat.Bc1RgbSrgbBlock
            TextureFormat.CompressedRgbaS3tcDxt1,         VkFormat.Bc1RgbaUnormBlock
            TextureFormat.CompressedSrgbAlphaS3tcDxt1,    VkFormat.Bc1RgbaSrgbBlock
            TextureFormat.CompressedRgbaS3tcDxt3,         VkFormat.Bc2UnormBlock
            TextureFormat.CompressedSrgbAlphaS3tcDxt3,    VkFormat.Bc2SrgbBlock
            TextureFormat.CompressedRgbaS3tcDxt5,         VkFormat.Bc3UnormBlock
            TextureFormat.CompressedSrgbAlphaS3tcDxt5,    VkFormat.Bc3SrgbBlock
            TextureFormat.CompressedRedRgtc1,             VkFormat.Bc4UnormBlock
            TextureFormat.CompressedSignedRedRgtc1,       VkFormat.Bc4SnormBlock
            TextureFormat.CompressedRgRgtc2,              VkFormat.Bc5UnormBlock
            TextureFormat.CompressedSignedRgRgtc2,        VkFormat.Bc5SnormBlock
            TextureFormat.CompressedRgbBptcSignedFloat,   VkFormat.Bc6hSfloatBlock
            TextureFormat.CompressedRgbBptcUnsignedFloat, VkFormat.Bc6hUfloatBlock
            TextureFormat.CompressedRgbaBptcUnorm,        VkFormat.Bc7UnormBlock
            TextureFormat.CompressedSrgbAlphaBptcUnorm,   VkFormat.Bc7SrgbBlock
        ]

    let toTextureFormat =
        let unknown = unbox<TextureFormat> 0
        LookupTable.lookup [
            VkFormat.Undefined, unknown
            VkFormat.R4g4UnormPack8, unknown
            VkFormat.R4g4b4a4UnormPack16, TextureFormat.Rgba4
            VkFormat.B4g4r4a4UnormPack16, unknown
            VkFormat.R5g6b5UnormPack16, unknown
            VkFormat.B5g6r5UnormPack16, unknown
            VkFormat.R5g5b5a1UnormPack16, unknown
            VkFormat.B5g5r5a1UnormPack16, unknown
            VkFormat.A1r5g5b5UnormPack16, unknown
            VkFormat.R8Unorm, TextureFormat.R8
            VkFormat.R8Snorm, TextureFormat.R8Snorm
            VkFormat.R8Uscaled, TextureFormat.R8
            VkFormat.R8Sscaled, TextureFormat.R8
            VkFormat.R8Uint, TextureFormat.R8ui
            VkFormat.R8Sint, TextureFormat.R8i
            VkFormat.R8Srgb, TextureFormat.R8
            VkFormat.R8g8Unorm, TextureFormat.Rg8
            VkFormat.R8g8Snorm, TextureFormat.Rg8Snorm
            VkFormat.R8g8Uscaled, TextureFormat.Rg8
            VkFormat.R8g8Sscaled, TextureFormat.Rg8
            VkFormat.R8g8Uint, TextureFormat.Rg8ui
            VkFormat.R8g8Sint, TextureFormat.Rg8i
            VkFormat.R8g8Srgb, TextureFormat.Rg8
            VkFormat.R8g8b8Unorm, TextureFormat.Rgb8
            VkFormat.R8g8b8Snorm, TextureFormat.Rgb8Snorm
            VkFormat.R8g8b8Uscaled, TextureFormat.Rgb8
            VkFormat.R8g8b8Sscaled, TextureFormat.Rgb8
            VkFormat.R8g8b8Uint, TextureFormat.Rgb8ui
            VkFormat.R8g8b8Sint, TextureFormat.Rgb8i
            VkFormat.R8g8b8Srgb, TextureFormat.Srgb8
            VkFormat.B8g8r8Unorm, TextureFormat.Bgr8
            VkFormat.B8g8r8Snorm, TextureFormat.Bgr8
            VkFormat.B8g8r8Uscaled, TextureFormat.Bgr8
            VkFormat.B8g8r8Sscaled, TextureFormat.Bgr8
            VkFormat.B8g8r8Uint, TextureFormat.Bgr8
            VkFormat.B8g8r8Sint, TextureFormat.Bgr8
            VkFormat.B8g8r8Srgb, TextureFormat.Bgr8
            VkFormat.R8g8b8a8Unorm, TextureFormat.Rgba8
            VkFormat.R8g8b8a8Snorm, TextureFormat.Rgba8Snorm
            VkFormat.R8g8b8a8Uscaled, TextureFormat.Rgba8
            VkFormat.R8g8b8a8Sscaled, TextureFormat.Rgba8
            VkFormat.R8g8b8a8Uint, TextureFormat.Rgba8ui
            VkFormat.R8g8b8a8Sint, TextureFormat.Rgba8i
            VkFormat.R8g8b8a8Srgb, TextureFormat.Srgb8Alpha8
            VkFormat.B8g8r8a8Unorm, TextureFormat.Bgra8
            VkFormat.B8g8r8a8Snorm, TextureFormat.Bgra8
            VkFormat.B8g8r8a8Uscaled, TextureFormat.Bgra8
            VkFormat.B8g8r8a8Sscaled, TextureFormat.Bgra8
            VkFormat.B8g8r8a8Uint, TextureFormat.Bgra8
            VkFormat.B8g8r8a8Sint, TextureFormat.Bgra8
            VkFormat.B8g8r8a8Srgb, TextureFormat.Bgra8
            VkFormat.A8b8g8r8UnormPack32, unknown
            VkFormat.A8b8g8r8SnormPack32, unknown
            VkFormat.A8b8g8r8UscaledPack32, unknown
            VkFormat.A8b8g8r8SscaledPack32, unknown
            VkFormat.A8b8g8r8UintPack32, unknown
            VkFormat.A8b8g8r8SintPack32, unknown
            VkFormat.A8b8g8r8SrgbPack32, unknown
            VkFormat.A2r10g10b10UnormPack32, unknown
            VkFormat.A2r10g10b10SnormPack32, unknown
            VkFormat.A2r10g10b10UscaledPack32, unknown
            VkFormat.A2r10g10b10SscaledPack32, unknown
            VkFormat.A2r10g10b10UintPack32, unknown
            VkFormat.A2r10g10b10SintPack32, unknown
            VkFormat.A2b10g10r10UnormPack32, unknown
            VkFormat.A2b10g10r10SnormPack32, unknown
            VkFormat.A2b10g10r10UscaledPack32, unknown
            VkFormat.A2b10g10r10SscaledPack32, unknown
            VkFormat.A2b10g10r10UintPack32, unknown
            VkFormat.A2b10g10r10SintPack32, unknown
            VkFormat.R16Unorm, TextureFormat.R16
            VkFormat.R16Snorm, TextureFormat.R16Snorm
            VkFormat.R16Uscaled, TextureFormat.R16
            VkFormat.R16Sscaled, TextureFormat.R16
            VkFormat.R16Uint, TextureFormat.R16ui
            VkFormat.R16Sint, TextureFormat.R16i
            VkFormat.R16Sfloat, TextureFormat.R16f
            VkFormat.R16g16Unorm, TextureFormat.Rg16
            VkFormat.R16g16Snorm, TextureFormat.Rg16Snorm
            VkFormat.R16g16Uscaled, TextureFormat.Rg16
            VkFormat.R16g16Sscaled, TextureFormat.Rg16
            VkFormat.R16g16Uint, TextureFormat.Rg16ui
            VkFormat.R16g16Sint, TextureFormat.Rg16i
            VkFormat.R16g16Sfloat, TextureFormat.Rg16f
            VkFormat.R16g16b16Unorm, TextureFormat.Rgb16
            VkFormat.R16g16b16Snorm, TextureFormat.Rgb16Snorm
            VkFormat.R16g16b16Uscaled, TextureFormat.Rgb16
            VkFormat.R16g16b16Sscaled, TextureFormat.Rgb16
            VkFormat.R16g16b16Uint, TextureFormat.Rgb16ui
            VkFormat.R16g16b16Sint, TextureFormat.Rgb16i
            VkFormat.R16g16b16Sfloat, TextureFormat.Rgb16f
            VkFormat.R16g16b16a16Unorm, TextureFormat.Rgba16
            VkFormat.R16g16b16a16Snorm, TextureFormat.Rgba16Snorm
            VkFormat.R16g16b16a16Uscaled, TextureFormat.Rgba16
            VkFormat.R16g16b16a16Sscaled, TextureFormat.Rgba16
            VkFormat.R16g16b16a16Uint, TextureFormat.Rgba16ui
            VkFormat.R16g16b16a16Sint, TextureFormat.Rgba16i
            VkFormat.R16g16b16a16Sfloat, TextureFormat.Rgba16f
            VkFormat.R32Uint, TextureFormat.R32ui
            VkFormat.R32Sint, TextureFormat.R32i
            VkFormat.R32Sfloat, TextureFormat.R32f
            VkFormat.R32g32Uint, TextureFormat.Rg32ui
            VkFormat.R32g32Sint, TextureFormat.Rg32i
            VkFormat.R32g32Sfloat, TextureFormat.Rg32f
            VkFormat.R32g32b32Uint, TextureFormat.Rgb32ui
            VkFormat.R32g32b32Sint, TextureFormat.Rgb32i
            VkFormat.R32g32b32Sfloat, TextureFormat.Rgb32f
            VkFormat.R32g32b32a32Uint, TextureFormat.Rgba32ui
            VkFormat.R32g32b32a32Sint, TextureFormat.Rgba32i
            VkFormat.R32g32b32a32Sfloat, TextureFormat.Rgba32f
            VkFormat.R64Uint, unknown
            VkFormat.R64Sint, unknown
            VkFormat.R64Sfloat, unknown
            VkFormat.R64g64Uint, unknown
            VkFormat.R64g64Sint, unknown
            VkFormat.R64g64Sfloat, unknown
            VkFormat.R64g64b64Uint, unknown
            VkFormat.R64g64b64Sint, unknown
            VkFormat.R64g64b64Sfloat, unknown
            VkFormat.R64g64b64a64Uint, unknown
            VkFormat.R64g64b64a64Sint, unknown
            VkFormat.R64g64b64a64Sfloat, unknown
            VkFormat.B10g11r11UfloatPack32, TextureFormat.R11fG11fB10f
            VkFormat.E5b9g9r9UfloatPack32, unknown
            VkFormat.D16Unorm, TextureFormat.DepthComponent16
            VkFormat.X8D24UnormPack32, TextureFormat.DepthComponent24
            VkFormat.D32Sfloat, TextureFormat.DepthComponent32f
            VkFormat.S8Uint, TextureFormat.StencilIndex8
            VkFormat.D16UnormS8Uint, unknown
            VkFormat.D24UnormS8Uint, TextureFormat.Depth24Stencil8
            VkFormat.D32SfloatS8Uint, TextureFormat.Depth32fStencil8
            VkFormat.Bc1RgbUnormBlock, TextureFormat.CompressedRgbS3tcDxt1
            VkFormat.Bc1RgbSrgbBlock, TextureFormat.CompressedSrgbS3tcDxt1
            VkFormat.Bc1RgbaUnormBlock, TextureFormat.CompressedRgbaS3tcDxt1
            VkFormat.Bc1RgbaSrgbBlock, TextureFormat.CompressedSrgbAlphaS3tcDxt1
            VkFormat.Bc2UnormBlock, TextureFormat.CompressedRgbaS3tcDxt3
            VkFormat.Bc2SrgbBlock, TextureFormat.CompressedSrgbAlphaS3tcDxt3
            VkFormat.Bc3UnormBlock, TextureFormat.CompressedRgbaS3tcDxt5
            VkFormat.Bc3SrgbBlock, TextureFormat.CompressedSrgbAlphaS3tcDxt5
            VkFormat.Bc4UnormBlock, TextureFormat.CompressedRedRgtc1
            VkFormat.Bc4SnormBlock, TextureFormat.CompressedSignedRedRgtc1
            VkFormat.Bc5UnormBlock, TextureFormat.CompressedRgRgtc2
            VkFormat.Bc5SnormBlock, TextureFormat.CompressedSignedRgRgtc2
            VkFormat.Bc6hSfloatBlock, TextureFormat.CompressedRgbBptcSignedFloat
            VkFormat.Bc6hUfloatBlock, TextureFormat.CompressedRgbBptcUnsignedFloat
            VkFormat.Bc7UnormBlock, TextureFormat.CompressedRgbaBptcUnorm
            VkFormat.Bc7SrgbBlock, TextureFormat.CompressedSrgbAlphaBptcUnorm
            VkFormat.Etc2R8g8b8UnormBlock, unknown
            VkFormat.Etc2R8g8b8SrgbBlock, unknown
            VkFormat.Etc2R8g8b8a1UnormBlock, unknown
            VkFormat.Etc2R8g8b8a1SrgbBlock, unknown
            VkFormat.Etc2R8g8b8a8UnormBlock, unknown
            VkFormat.Etc2R8g8b8a8SrgbBlock, unknown
            VkFormat.EacR11UnormBlock, unknown
            VkFormat.EacR11SnormBlock, unknown
            VkFormat.EacR11g11UnormBlock, unknown
            VkFormat.EacR11g11SnormBlock, unknown
            VkFormat.Astc44UnormBlock, unknown
            VkFormat.Astc44SrgbBlock, unknown
            VkFormat.Astc54UnormBlock, unknown
            VkFormat.Astc54SrgbBlock, unknown
            VkFormat.Astc55UnormBlock, unknown
            VkFormat.Astc55SrgbBlock, unknown
            VkFormat.Astc65UnormBlock, unknown
            VkFormat.Astc65SrgbBlock, unknown
            VkFormat.Astc66UnormBlock, unknown
            VkFormat.Astc66SrgbBlock, unknown
            VkFormat.Astc85UnormBlock, unknown
            VkFormat.Astc85SrgbBlock, unknown
            VkFormat.Astc86UnormBlock, unknown
            VkFormat.Astc86SrgbBlock, unknown
            VkFormat.Astc88UnormBlock, unknown
            VkFormat.Astc88SrgbBlock, unknown
            VkFormat.Astc105UnormBlock, unknown
            VkFormat.Astc105SrgbBlock, unknown
            VkFormat.Astc106UnormBlock, unknown
            VkFormat.Astc106SrgbBlock, unknown
            VkFormat.Astc108UnormBlock, unknown
            VkFormat.Astc108SrgbBlock, unknown
            VkFormat.Astc1010UnormBlock, unknown
            VkFormat.Astc1010SrgbBlock, unknown
            VkFormat.Astc1210UnormBlock, unknown
            VkFormat.Astc1210SrgbBlock, unknown
            VkFormat.Astc1212UnormBlock, unknown
            VkFormat.Astc1212SrgbBlock, unknown
        ]

    let pixelSizeInBytes =
        LookupTable.lookup [
            VkFormat.Undefined, 0
            VkFormat.R4g4UnormPack8, 1
            VkFormat.R4g4b4a4UnormPack16, 2
            VkFormat.B4g4r4a4UnormPack16, 2
            VkFormat.R5g6b5UnormPack16, 2
            VkFormat.B5g6r5UnormPack16, 2
            VkFormat.R5g5b5a1UnormPack16, 2
            VkFormat.B5g5r5a1UnormPack16, 2
            VkFormat.A1r5g5b5UnormPack16, 2
            VkFormat.R8Unorm, 1
            VkFormat.R8Snorm, 1
            VkFormat.R8Uscaled, 1
            VkFormat.R8Sscaled, 1
            VkFormat.R8Uint, 1
            VkFormat.R8Sint, 1
            VkFormat.R8Srgb, 1
            VkFormat.R8g8Unorm, 2
            VkFormat.R8g8Snorm, 2
            VkFormat.R8g8Uscaled, 2
            VkFormat.R8g8Sscaled, 2
            VkFormat.R8g8Uint, 2
            VkFormat.R8g8Sint, 2
            VkFormat.R8g8Srgb, 2
            VkFormat.R8g8b8Unorm, 3
            VkFormat.R8g8b8Snorm, 3
            VkFormat.R8g8b8Uscaled, 3
            VkFormat.R8g8b8Sscaled, 3
            VkFormat.R8g8b8Uint, 3
            VkFormat.R8g8b8Sint, 3
            VkFormat.R8g8b8Srgb, 3
            VkFormat.B8g8r8Unorm, 3
            VkFormat.B8g8r8Snorm, 3
            VkFormat.B8g8r8Uscaled, 3
            VkFormat.B8g8r8Sscaled, 3
            VkFormat.B8g8r8Uint, 3
            VkFormat.B8g8r8Sint, 3
            VkFormat.B8g8r8Srgb, 3
            VkFormat.R8g8b8a8Unorm, 4
            VkFormat.R8g8b8a8Snorm, 4
            VkFormat.R8g8b8a8Uscaled, 4
            VkFormat.R8g8b8a8Sscaled, 4
            VkFormat.R8g8b8a8Uint, 4
            VkFormat.R8g8b8a8Sint, 4
            VkFormat.R8g8b8a8Srgb, 4
            VkFormat.B8g8r8a8Unorm, 4
            VkFormat.B8g8r8a8Snorm, 4
            VkFormat.B8g8r8a8Uscaled, 4
            VkFormat.B8g8r8a8Sscaled, 4
            VkFormat.B8g8r8a8Uint, 4
            VkFormat.B8g8r8a8Sint, 4
            VkFormat.B8g8r8a8Srgb, 4
            VkFormat.A8b8g8r8UnormPack32, 4
            VkFormat.A8b8g8r8SnormPack32, 4
            VkFormat.A8b8g8r8UscaledPack32, 4
            VkFormat.A8b8g8r8SscaledPack32, 4
            VkFormat.A8b8g8r8UintPack32, 4
            VkFormat.A8b8g8r8SintPack32, 4
            VkFormat.A8b8g8r8SrgbPack32, 4
            VkFormat.A2r10g10b10UnormPack32, 4
            VkFormat.A2r10g10b10SnormPack32, 4
            VkFormat.A2r10g10b10UscaledPack32, 4
            VkFormat.A2r10g10b10SscaledPack32, 4
            VkFormat.A2r10g10b10UintPack32, 4
            VkFormat.A2r10g10b10SintPack32, 4
            VkFormat.A2b10g10r10UnormPack32, 4
            VkFormat.A2b10g10r10SnormPack32, 4
            VkFormat.A2b10g10r10UscaledPack32, 4
            VkFormat.A2b10g10r10SscaledPack32, 4
            VkFormat.A2b10g10r10UintPack32, 4
            VkFormat.A2b10g10r10SintPack32, 4
            VkFormat.R16Unorm, 2
            VkFormat.R16Snorm, 2
            VkFormat.R16Uscaled, 2
            VkFormat.R16Sscaled, 2
            VkFormat.R16Uint, 2
            VkFormat.R16Sint, 2
            VkFormat.R16Sfloat, 2
            VkFormat.R16g16Unorm, 4
            VkFormat.R16g16Snorm, 4
            VkFormat.R16g16Uscaled, 4
            VkFormat.R16g16Sscaled, 4
            VkFormat.R16g16Uint, 4
            VkFormat.R16g16Sint, 4
            VkFormat.R16g16Sfloat, 4
            VkFormat.R16g16b16Unorm, 6
            VkFormat.R16g16b16Snorm, 6
            VkFormat.R16g16b16Uscaled, 6
            VkFormat.R16g16b16Sscaled, 6
            VkFormat.R16g16b16Uint, 6
            VkFormat.R16g16b16Sint, 6
            VkFormat.R16g16b16Sfloat, 6
            VkFormat.R16g16b16a16Unorm, 8
            VkFormat.R16g16b16a16Snorm, 8
            VkFormat.R16g16b16a16Uscaled, 8
            VkFormat.R16g16b16a16Sscaled, 8
            VkFormat.R16g16b16a16Uint, 8
            VkFormat.R16g16b16a16Sint, 8
            VkFormat.R16g16b16a16Sfloat, 8
            VkFormat.R32Uint, 4
            VkFormat.R32Sint, 4
            VkFormat.R32Sfloat, 4
            VkFormat.R32g32Uint, 8
            VkFormat.R32g32Sint, 8
            VkFormat.R32g32Sfloat, 8
            VkFormat.R32g32b32Uint, 12
            VkFormat.R32g32b32Sint, 12
            VkFormat.R32g32b32Sfloat, 12
            VkFormat.R32g32b32a32Uint, 16
            VkFormat.R32g32b32a32Sint, 16
            VkFormat.R32g32b32a32Sfloat, 16
            VkFormat.R64Uint, 8
            VkFormat.R64Sint, 8
            VkFormat.R64Sfloat, 8
            VkFormat.R64g64Uint, 16
            VkFormat.R64g64Sint, 16
            VkFormat.R64g64Sfloat, 16
            VkFormat.R64g64b64Uint, 24
            VkFormat.R64g64b64Sint, 24
            VkFormat.R64g64b64Sfloat, 24
            VkFormat.R64g64b64a64Uint, 32
            VkFormat.R64g64b64a64Sint, 32
            VkFormat.R64g64b64a64Sfloat, 32
            VkFormat.B10g11r11UfloatPack32, 4
            VkFormat.E5b9g9r9UfloatPack32, 4
            VkFormat.D16Unorm, 2
            VkFormat.X8D24UnormPack32, 4
            VkFormat.D32Sfloat, 4
            VkFormat.S8Uint, 1
            VkFormat.D16UnormS8Uint, 3
            VkFormat.D24UnormS8Uint, 4
            VkFormat.D32SfloatS8Uint, 5
            VkFormat.Bc1RgbUnormBlock, 0
            VkFormat.Bc1RgbSrgbBlock, 0
            VkFormat.Bc1RgbaUnormBlock, 0
            VkFormat.Bc1RgbaSrgbBlock, 0
            VkFormat.Bc2UnormBlock, 0
            VkFormat.Bc2SrgbBlock, 0
            VkFormat.Bc3UnormBlock, 0
            VkFormat.Bc3SrgbBlock, 0
            VkFormat.Bc4UnormBlock, 0
            VkFormat.Bc4SnormBlock, 0
            VkFormat.Bc5UnormBlock, 0
            VkFormat.Bc5SnormBlock, 0
            VkFormat.Bc6hUfloatBlock, 0
            VkFormat.Bc6hSfloatBlock, 0
            VkFormat.Bc7UnormBlock, 0
            VkFormat.Bc7SrgbBlock, 0
            VkFormat.Etc2R8g8b8UnormBlock, 0
            VkFormat.Etc2R8g8b8SrgbBlock, 0
            VkFormat.Etc2R8g8b8a1UnormBlock, 0
            VkFormat.Etc2R8g8b8a1SrgbBlock, 0
            VkFormat.Etc2R8g8b8a8UnormBlock, 0
            VkFormat.Etc2R8g8b8a8SrgbBlock, 0
            VkFormat.EacR11UnormBlock, 0
            VkFormat.EacR11SnormBlock, 0
            VkFormat.EacR11g11UnormBlock, 0
            VkFormat.EacR11g11SnormBlock, 0
            VkFormat.Astc44UnormBlock, 0
            VkFormat.Astc44SrgbBlock, 0
            VkFormat.Astc54UnormBlock, 0
            VkFormat.Astc54SrgbBlock, 0
            VkFormat.Astc55UnormBlock, 0
            VkFormat.Astc55SrgbBlock, 0
            VkFormat.Astc65UnormBlock, 0
            VkFormat.Astc65SrgbBlock, 0
            VkFormat.Astc66UnormBlock, 0
            VkFormat.Astc66SrgbBlock, 0
            VkFormat.Astc85UnormBlock, 0
            VkFormat.Astc85SrgbBlock, 0
            VkFormat.Astc86UnormBlock, 0
            VkFormat.Astc86SrgbBlock, 0
            VkFormat.Astc88UnormBlock, 0
            VkFormat.Astc88SrgbBlock, 0
            VkFormat.Astc105UnormBlock, 0
            VkFormat.Astc105SrgbBlock, 0
            VkFormat.Astc106UnormBlock, 0
            VkFormat.Astc106SrgbBlock, 0
            VkFormat.Astc108UnormBlock, 0
            VkFormat.Astc108SrgbBlock, 0
            VkFormat.Astc1010UnormBlock, 0
            VkFormat.Astc1010SrgbBlock, 0
            VkFormat.Astc1210UnormBlock, 0
            VkFormat.Astc1210SrgbBlock, 0
            VkFormat.Astc1212UnormBlock, 0
            VkFormat.Astc1212SrgbBlock, 0
        ]

    let private depthFormats = HashSet.ofList [ VkFormat.D16Unorm; VkFormat.D32Sfloat; VkFormat.X8D24UnormPack32 ]
    let private stencilFormats = HashSet.ofList [ VkFormat.S8Uint ]
    let private depthStencilFormats = HashSet.ofList [VkFormat.D16UnormS8Uint; VkFormat.D24UnormS8Uint; VkFormat.D32SfloatS8Uint ]

    let hasDepth (fmt : VkFormat) =
        depthFormats.Contains fmt || depthStencilFormats.Contains fmt

    let hasStencil (fmt : VkFormat) =
        stencilFormats.Contains fmt || depthStencilFormats.Contains fmt

    let private srgbFormats =
        HashSet.ofList [
            VkFormat.R8Srgb
            VkFormat.R8g8Srgb
            VkFormat.R8g8b8Srgb
            VkFormat.B8g8r8Srgb
            VkFormat.R8g8b8a8Srgb
            VkFormat.B8g8r8a8Srgb
            VkFormat.A8b8g8r8SrgbPack32
            VkFormat.Bc1RgbSrgbBlock
            VkFormat.Bc1RgbaSrgbBlock
            VkFormat.Bc2SrgbBlock
            VkFormat.Bc3SrgbBlock
            VkFormat.Bc7SrgbBlock
            VkFormat.Etc2R8g8b8SrgbBlock
            VkFormat.Etc2R8g8b8a1SrgbBlock
            VkFormat.Etc2R8g8b8a8SrgbBlock
            VkFormat.Astc44SrgbBlock
            VkFormat.Astc54SrgbBlock
            VkFormat.Astc55SrgbBlock
            VkFormat.Astc65SrgbBlock
            VkFormat.Astc66SrgbBlock
            VkFormat.Astc85SrgbBlock
            VkFormat.Astc86SrgbBlock
            VkFormat.Astc88SrgbBlock
            VkFormat.Astc105SrgbBlock
            VkFormat.Astc106SrgbBlock
            VkFormat.Astc108SrgbBlock
            VkFormat.Astc1010SrgbBlock
            VkFormat.Astc1210SrgbBlock
            VkFormat.Astc1212SrgbBlock
        ]

    let isSrgb (fmt : VkFormat) =
        srgbFormats.Contains fmt

    let toAspect (fmt : VkFormat) =
        if depthStencilFormats.Contains fmt then VkImageAspectFlags.DepthBit ||| VkImageAspectFlags.StencilBit
        elif depthFormats.Contains fmt then VkImageAspectFlags.DepthBit
        elif stencilFormats.Contains fmt then VkImageAspectFlags.StencilBit
        else VkImageAspectFlags.ColorBit

    // Same as toAspect but returns only the depth aspect for depth / stencil formats.
    // An image view for a depth / stencil image used in shaders (e.g. sampler) can only either have depth or stencil aspect but not both.
    let toShaderAspect (fmt : VkFormat) =
        if depthStencilFormats.Contains fmt then VkImageAspectFlags.DepthBit
        elif depthFormats.Contains fmt then VkImageAspectFlags.DepthBit
        elif stencilFormats.Contains fmt then VkImageAspectFlags.StencilBit
        else VkImageAspectFlags.ColorBit

    let toColFormat =
        let r = Col.Format.Gray
        let rg = Col.Format.RG
        let rgb = Col.Format.RGB
        let rgba = Col.Format.RGBA
        let bgr = Col.Format.BGR
        let bgra = Col.Format.BGRA
        let argb = Col.Format.None
        let abgr = Col.Format.None
        let none = Col.Format.None
        let d = Col.Format.Gray
        let ds = Col.Format.GrayAlpha
        let s = Col.Format.Alpha
        let unknown = Col.Format.None
        LookupTable.lookup [
            VkFormat.Undefined, none
            VkFormat.R4g4UnormPack8, rg
            VkFormat.R4g4b4a4UnormPack16, rgba
            VkFormat.B4g4r4a4UnormPack16, bgra
            VkFormat.R5g6b5UnormPack16, rgb
            VkFormat.B5g6r5UnormPack16, bgr
            VkFormat.R5g5b5a1UnormPack16, rgba
            VkFormat.B5g5r5a1UnormPack16, bgra
            VkFormat.A1r5g5b5UnormPack16, argb
            VkFormat.R8Unorm, r
            VkFormat.R8Snorm, r
            VkFormat.R8Uscaled, r
            VkFormat.R8Sscaled, r
            VkFormat.R8Uint, r
            VkFormat.R8Sint, r
            VkFormat.R8Srgb, r
            VkFormat.R8g8Unorm, rg
            VkFormat.R8g8Snorm, rg
            VkFormat.R8g8Uscaled, rg
            VkFormat.R8g8Sscaled, rg
            VkFormat.R8g8Uint, rg
            VkFormat.R8g8Sint, rg
            VkFormat.R8g8Srgb, rg
            VkFormat.R8g8b8Unorm, rgb
            VkFormat.R8g8b8Snorm, rgb
            VkFormat.R8g8b8Uscaled, rgb
            VkFormat.R8g8b8Sscaled, rgb
            VkFormat.R8g8b8Uint, rgb
            VkFormat.R8g8b8Sint, rgb
            VkFormat.R8g8b8Srgb, rgb
            VkFormat.B8g8r8Unorm, bgr
            VkFormat.B8g8r8Snorm, bgr
            VkFormat.B8g8r8Uscaled, bgr
            VkFormat.B8g8r8Sscaled, bgr
            VkFormat.B8g8r8Uint, bgr
            VkFormat.B8g8r8Sint, bgr
            VkFormat.B8g8r8Srgb, bgr
            VkFormat.R8g8b8a8Unorm, rgba
            VkFormat.R8g8b8a8Snorm, rgba
            VkFormat.R8g8b8a8Uscaled, rgba
            VkFormat.R8g8b8a8Sscaled, rgba
            VkFormat.R8g8b8a8Uint, rgba
            VkFormat.R8g8b8a8Sint, rgba
            VkFormat.R8g8b8a8Srgb, rgba
            VkFormat.B8g8r8a8Unorm, bgra
            VkFormat.B8g8r8a8Snorm, bgra
            VkFormat.B8g8r8a8Uscaled, bgra
            VkFormat.B8g8r8a8Sscaled, bgra
            VkFormat.B8g8r8a8Uint, bgra
            VkFormat.B8g8r8a8Sint, bgra
            VkFormat.B8g8r8a8Srgb, bgra
            VkFormat.A8b8g8r8UnormPack32, abgr
            VkFormat.A8b8g8r8SnormPack32, abgr
            VkFormat.A8b8g8r8UscaledPack32, abgr
            VkFormat.A8b8g8r8SscaledPack32, abgr
            VkFormat.A8b8g8r8UintPack32, abgr
            VkFormat.A8b8g8r8SintPack32, abgr
            VkFormat.A8b8g8r8SrgbPack32, abgr
            VkFormat.A2r10g10b10UnormPack32, argb
            VkFormat.A2r10g10b10SnormPack32, argb
            VkFormat.A2r10g10b10UscaledPack32, argb
            VkFormat.A2r10g10b10SscaledPack32, argb
            VkFormat.A2r10g10b10UintPack32, argb
            VkFormat.A2r10g10b10SintPack32, argb
            VkFormat.A2b10g10r10UnormPack32, abgr
            VkFormat.A2b10g10r10SnormPack32, abgr
            VkFormat.A2b10g10r10UscaledPack32, abgr
            VkFormat.A2b10g10r10SscaledPack32, abgr
            VkFormat.A2b10g10r10UintPack32, abgr
            VkFormat.A2b10g10r10SintPack32, abgr
            VkFormat.R16Unorm, r
            VkFormat.R16Snorm, r
            VkFormat.R16Uscaled, r
            VkFormat.R16Sscaled, r
            VkFormat.R16Uint, r
            VkFormat.R16Sint, r
            VkFormat.R16Sfloat, r
            VkFormat.R16g16Unorm, rg
            VkFormat.R16g16Snorm, rg
            VkFormat.R16g16Uscaled, rg
            VkFormat.R16g16Sscaled, rg
            VkFormat.R16g16Uint, rg
            VkFormat.R16g16Sint, rg
            VkFormat.R16g16Sfloat, rg
            VkFormat.R16g16b16Unorm, rgb
            VkFormat.R16g16b16Snorm, rgb
            VkFormat.R16g16b16Uscaled, rgb
            VkFormat.R16g16b16Sscaled, rgb
            VkFormat.R16g16b16Uint, rgb
            VkFormat.R16g16b16Sint, rgb
            VkFormat.R16g16b16Sfloat, rgb
            VkFormat.R16g16b16a16Unorm, rgba
            VkFormat.R16g16b16a16Snorm, rgba
            VkFormat.R16g16b16a16Uscaled, rgba
            VkFormat.R16g16b16a16Sscaled, rgba
            VkFormat.R16g16b16a16Uint, rgba
            VkFormat.R16g16b16a16Sint, rgba
            VkFormat.R16g16b16a16Sfloat, rgba
            VkFormat.R32Uint, r
            VkFormat.R32Sint, r
            VkFormat.R32Sfloat, r
            VkFormat.R32g32Uint, rg
            VkFormat.R32g32Sint, rg
            VkFormat.R32g32Sfloat, rg
            VkFormat.R32g32b32Uint, rgb
            VkFormat.R32g32b32Sint, rgb
            VkFormat.R32g32b32Sfloat, rgb
            VkFormat.R32g32b32a32Uint, rgba
            VkFormat.R32g32b32a32Sint, rgba
            VkFormat.R32g32b32a32Sfloat, rgba
            VkFormat.R64Uint, r
            VkFormat.R64Sint, r
            VkFormat.R64Sfloat, r
            VkFormat.R64g64Uint, rg
            VkFormat.R64g64Sint, rg
            VkFormat.R64g64Sfloat, rg
            VkFormat.R64g64b64Uint, rgb
            VkFormat.R64g64b64Sint, rgb
            VkFormat.R64g64b64Sfloat, rgb
            VkFormat.R64g64b64a64Uint, rgba
            VkFormat.R64g64b64a64Sint, rgba
            VkFormat.R64g64b64a64Sfloat, rgba
            VkFormat.B10g11r11UfloatPack32, bgr
            VkFormat.E5b9g9r9UfloatPack32, bgr
            VkFormat.D16Unorm, d
            VkFormat.X8D24UnormPack32, d
            VkFormat.D32Sfloat, ds
            VkFormat.S8Uint, s
            VkFormat.D16UnormS8Uint, ds
            VkFormat.D24UnormS8Uint, ds
            VkFormat.D32SfloatS8Uint, ds
            VkFormat.Bc1RgbUnormBlock, rgb
            VkFormat.Bc1RgbSrgbBlock, rgb
            VkFormat.Bc1RgbaUnormBlock, rgba
            VkFormat.Bc1RgbaSrgbBlock, rgba
            VkFormat.Bc2UnormBlock, rgba
            VkFormat.Bc2SrgbBlock, rgba
            VkFormat.Bc3UnormBlock, rgba
            VkFormat.Bc3SrgbBlock, rgba
            VkFormat.Bc4UnormBlock, r
            VkFormat.Bc4SnormBlock, r
            VkFormat.Bc5UnormBlock, rg
            VkFormat.Bc5SnormBlock, rg
            VkFormat.Bc6hUfloatBlock, rgb
            VkFormat.Bc6hSfloatBlock, rgb
            VkFormat.Bc7UnormBlock, rgba
            VkFormat.Bc7SrgbBlock, rgba
            VkFormat.Etc2R8g8b8UnormBlock, rgb
            VkFormat.Etc2R8g8b8SrgbBlock, rgb
            VkFormat.Etc2R8g8b8a1UnormBlock, rgba
            VkFormat.Etc2R8g8b8a1SrgbBlock, rgba
            VkFormat.Etc2R8g8b8a8UnormBlock, rgba
            VkFormat.Etc2R8g8b8a8SrgbBlock, rgba
            VkFormat.EacR11UnormBlock, r
            VkFormat.EacR11SnormBlock, r
            VkFormat.EacR11g11UnormBlock, rg
            VkFormat.EacR11g11SnormBlock, rg
            VkFormat.Astc44UnormBlock, unknown
            VkFormat.Astc44SrgbBlock, rgb
            VkFormat.Astc54UnormBlock, unknown
            VkFormat.Astc54SrgbBlock, rgb
            VkFormat.Astc55UnormBlock, unknown
            VkFormat.Astc55SrgbBlock, rgb
            VkFormat.Astc65UnormBlock, unknown
            VkFormat.Astc65SrgbBlock, rgb
            VkFormat.Astc66UnormBlock, unknown
            VkFormat.Astc66SrgbBlock, rgb
            VkFormat.Astc85UnormBlock, unknown
            VkFormat.Astc85SrgbBlock, rgb
            VkFormat.Astc86UnormBlock, unknown
            VkFormat.Astc86SrgbBlock, rgb
            VkFormat.Astc88UnormBlock, unknown
            VkFormat.Astc88SrgbBlock, rgb
            VkFormat.Astc105UnormBlock, unknown
            VkFormat.Astc105SrgbBlock, rgb
            VkFormat.Astc106UnormBlock, unknown
            VkFormat.Astc106SrgbBlock, rgb
            VkFormat.Astc108UnormBlock, unknown
            VkFormat.Astc108SrgbBlock, rgb
            VkFormat.Astc1010UnormBlock, unknown
            VkFormat.Astc1010SrgbBlock, rgb
            VkFormat.Astc1210UnormBlock, unknown
            VkFormat.Astc1210SrgbBlock, rgb
            VkFormat.Astc1212UnormBlock, unknown
            VkFormat.Astc1212SrgbBlock, rgb
        ]

    let channels =
        LookupTable.lookup [
            VkFormat.Undefined, -1
            VkFormat.R4g4UnormPack8, 2
            VkFormat.R4g4b4a4UnormPack16, 4
            VkFormat.B4g4r4a4UnormPack16, 4
            VkFormat.R5g6b5UnormPack16, 3
            VkFormat.B5g6r5UnormPack16, 3
            VkFormat.R5g5b5a1UnormPack16, 4
            VkFormat.B5g5r5a1UnormPack16, 4
            VkFormat.A1r5g5b5UnormPack16, 4
            VkFormat.R8Unorm, 1
            VkFormat.R8Snorm, 1
            VkFormat.R8Uscaled, 1
            VkFormat.R8Sscaled, 1
            VkFormat.R8Uint, 1
            VkFormat.R8Sint, 1
            VkFormat.R8Srgb, 1
            VkFormat.R8g8Unorm, 2
            VkFormat.R8g8Snorm, 2
            VkFormat.R8g8Uscaled, 2
            VkFormat.R8g8Sscaled, 2
            VkFormat.R8g8Uint, 2
            VkFormat.R8g8Sint, 2
            VkFormat.R8g8Srgb, 2
            VkFormat.R8g8b8Unorm, 3
            VkFormat.R8g8b8Snorm, 3
            VkFormat.R8g8b8Uscaled, 3
            VkFormat.R8g8b8Sscaled, 3
            VkFormat.R8g8b8Uint, 3
            VkFormat.R8g8b8Sint, 3
            VkFormat.R8g8b8Srgb, 3
            VkFormat.B8g8r8Unorm, 3
            VkFormat.B8g8r8Snorm, 3
            VkFormat.B8g8r8Uscaled, 3
            VkFormat.B8g8r8Sscaled, 3
            VkFormat.B8g8r8Uint, 3
            VkFormat.B8g8r8Sint, 3
            VkFormat.B8g8r8Srgb, 3
            VkFormat.R8g8b8a8Unorm, 4
            VkFormat.R8g8b8a8Snorm, 4
            VkFormat.R8g8b8a8Uscaled, 4
            VkFormat.R8g8b8a8Sscaled, 4
            VkFormat.R8g8b8a8Uint, 4
            VkFormat.R8g8b8a8Sint, 4
            VkFormat.R8g8b8a8Srgb, 4
            VkFormat.B8g8r8a8Unorm, 4
            VkFormat.B8g8r8a8Snorm, 4
            VkFormat.B8g8r8a8Uscaled, 4
            VkFormat.B8g8r8a8Sscaled, 4
            VkFormat.B8g8r8a8Uint, 4
            VkFormat.B8g8r8a8Sint, 4
            VkFormat.B8g8r8a8Srgb, 4
            VkFormat.A8b8g8r8UnormPack32, 4
            VkFormat.A8b8g8r8SnormPack32, 4
            VkFormat.A8b8g8r8UscaledPack32, 4
            VkFormat.A8b8g8r8SscaledPack32, 4
            VkFormat.A8b8g8r8UintPack32, 4
            VkFormat.A8b8g8r8SintPack32, 4
            VkFormat.A8b8g8r8SrgbPack32, 4
            VkFormat.A2r10g10b10UnormPack32, 4
            VkFormat.A2r10g10b10SnormPack32, 4
            VkFormat.A2r10g10b10UscaledPack32, 4
            VkFormat.A2r10g10b10SscaledPack32, 4
            VkFormat.A2r10g10b10UintPack32, 4
            VkFormat.A2r10g10b10SintPack32, 4
            VkFormat.A2b10g10r10UnormPack32, 4
            VkFormat.A2b10g10r10SnormPack32, 4
            VkFormat.A2b10g10r10UscaledPack32, 4
            VkFormat.A2b10g10r10SscaledPack32, 4
            VkFormat.A2b10g10r10UintPack32, 4
            VkFormat.A2b10g10r10SintPack32, 4
            VkFormat.R16Unorm, 1
            VkFormat.R16Snorm, 1
            VkFormat.R16Uscaled, 1
            VkFormat.R16Sscaled, 1
            VkFormat.R16Uint, 1
            VkFormat.R16Sint, 1
            VkFormat.R16Sfloat, 1
            VkFormat.R16g16Unorm, 2
            VkFormat.R16g16Snorm, 2
            VkFormat.R16g16Uscaled, 2
            VkFormat.R16g16Sscaled, 2
            VkFormat.R16g16Uint, 2
            VkFormat.R16g16Sint, 2
            VkFormat.R16g16Sfloat, 2
            VkFormat.R16g16b16Unorm, 3
            VkFormat.R16g16b16Snorm, 3
            VkFormat.R16g16b16Uscaled, 3
            VkFormat.R16g16b16Sscaled, 3
            VkFormat.R16g16b16Uint, 3
            VkFormat.R16g16b16Sint, 3
            VkFormat.R16g16b16Sfloat, 3
            VkFormat.R16g16b16a16Unorm, 4
            VkFormat.R16g16b16a16Snorm, 4
            VkFormat.R16g16b16a16Uscaled, 4
            VkFormat.R16g16b16a16Sscaled, 4
            VkFormat.R16g16b16a16Uint, 4
            VkFormat.R16g16b16a16Sint, 4
            VkFormat.R16g16b16a16Sfloat, 4
            VkFormat.R32Uint, 1
            VkFormat.R32Sint, 1
            VkFormat.R32Sfloat, 1
            VkFormat.R32g32Uint, 2
            VkFormat.R32g32Sint, 2
            VkFormat.R32g32Sfloat, 2
            VkFormat.R32g32b32Uint, 3
            VkFormat.R32g32b32Sint, 3
            VkFormat.R32g32b32Sfloat, 3
            VkFormat.R32g32b32a32Uint, 4
            VkFormat.R32g32b32a32Sint, 4
            VkFormat.R32g32b32a32Sfloat, 4
            VkFormat.R64Uint, 1
            VkFormat.R64Sint, 1
            VkFormat.R64Sfloat, 1
            VkFormat.R64g64Uint, 2
            VkFormat.R64g64Sint, 2
            VkFormat.R64g64Sfloat, 2
            VkFormat.R64g64b64Uint, 3
            VkFormat.R64g64b64Sint, 3
            VkFormat.R64g64b64Sfloat, 3
            VkFormat.R64g64b64a64Uint, 4
            VkFormat.R64g64b64a64Sint, 4
            VkFormat.R64g64b64a64Sfloat, 4
            VkFormat.B10g11r11UfloatPack32, 3
            VkFormat.E5b9g9r9UfloatPack32, 3
            VkFormat.D16Unorm, 1
            VkFormat.X8D24UnormPack32, 1
            VkFormat.D32Sfloat, 1
            VkFormat.S8Uint, 1
            VkFormat.D16UnormS8Uint, 2
            VkFormat.D24UnormS8Uint, 2
            VkFormat.D32SfloatS8Uint, 2
            VkFormat.Bc1RgbUnormBlock, -1
            VkFormat.Bc1RgbSrgbBlock, -1
            VkFormat.Bc1RgbaUnormBlock, -1
            VkFormat.Bc1RgbaSrgbBlock, -1
            VkFormat.Bc2UnormBlock, -1
            VkFormat.Bc2SrgbBlock, -1
            VkFormat.Bc3UnormBlock, -1
            VkFormat.Bc3SrgbBlock, -1
            VkFormat.Bc4UnormBlock, -1
            VkFormat.Bc4SnormBlock, -1
            VkFormat.Bc5UnormBlock, -1
            VkFormat.Bc5SnormBlock, -1
            VkFormat.Bc6hUfloatBlock, -1
            VkFormat.Bc6hSfloatBlock, -1
            VkFormat.Bc7UnormBlock, -1
            VkFormat.Bc7SrgbBlock, -1
            VkFormat.Etc2R8g8b8UnormBlock, -1
            VkFormat.Etc2R8g8b8SrgbBlock, -1
            VkFormat.Etc2R8g8b8a1UnormBlock, -1
            VkFormat.Etc2R8g8b8a1SrgbBlock, -1
            VkFormat.Etc2R8g8b8a8UnormBlock, -1
            VkFormat.Etc2R8g8b8a8SrgbBlock, -1
            VkFormat.EacR11UnormBlock, -1
            VkFormat.EacR11SnormBlock, -1
            VkFormat.EacR11g11UnormBlock, -1
            VkFormat.EacR11g11SnormBlock, -1
            VkFormat.Astc44UnormBlock, -1
            VkFormat.Astc44SrgbBlock, -1
            VkFormat.Astc54UnormBlock, -1
            VkFormat.Astc54SrgbBlock, -1
            VkFormat.Astc55UnormBlock, -1
            VkFormat.Astc55SrgbBlock, -1
            VkFormat.Astc65UnormBlock, -1
            VkFormat.Astc65SrgbBlock, -1
            VkFormat.Astc66UnormBlock, -1
            VkFormat.Astc66SrgbBlock, -1
            VkFormat.Astc85UnormBlock, -1
            VkFormat.Astc85SrgbBlock, -1
            VkFormat.Astc86UnormBlock, -1
            VkFormat.Astc86SrgbBlock, -1
            VkFormat.Astc88UnormBlock, -1
            VkFormat.Astc88SrgbBlock, -1
            VkFormat.Astc105UnormBlock, -1
            VkFormat.Astc105SrgbBlock, -1
            VkFormat.Astc106UnormBlock, -1
            VkFormat.Astc106SrgbBlock, -1
            VkFormat.Astc108UnormBlock, -1
            VkFormat.Astc108SrgbBlock, -1
            VkFormat.Astc1010UnormBlock, -1
            VkFormat.Astc1010SrgbBlock, -1
            VkFormat.Astc1210UnormBlock, -1
            VkFormat.Astc1210SrgbBlock, -1
            VkFormat.Astc1212UnormBlock, -1
            VkFormat.Astc1212SrgbBlock, -1
        ]

    let sizeInBytes =

        LookupTable.lookup [
            VkFormat.Undefined, -1
            VkFormat.R4g4UnormPack8, 1
            VkFormat.R4g4b4a4UnormPack16, 2
            VkFormat.B4g4r4a4UnormPack16, 2
            VkFormat.R5g6b5UnormPack16, 2
            VkFormat.B5g6r5UnormPack16, 2
            VkFormat.R5g5b5a1UnormPack16, 2
            VkFormat.B5g5r5a1UnormPack16, 2
            VkFormat.A1r5g5b5UnormPack16, 2
            VkFormat.R8Unorm, 1
            VkFormat.R8Snorm, 1
            VkFormat.R8Uscaled, 1
            VkFormat.R8Sscaled, 1
            VkFormat.R8Uint, 1
            VkFormat.R8Sint, 1
            VkFormat.R8Srgb, 1
            VkFormat.R8g8Unorm, 2
            VkFormat.R8g8Snorm, 2
            VkFormat.R8g8Uscaled, 2
            VkFormat.R8g8Sscaled, 2
            VkFormat.R8g8Uint, 2
            VkFormat.R8g8Sint, 2
            VkFormat.R8g8Srgb, 2
            VkFormat.R8g8b8Unorm, 3
            VkFormat.R8g8b8Snorm, 3
            VkFormat.R8g8b8Uscaled, 3
            VkFormat.R8g8b8Sscaled, 3
            VkFormat.R8g8b8Uint, 3
            VkFormat.R8g8b8Sint, 3
            VkFormat.R8g8b8Srgb, 3
            VkFormat.B8g8r8Unorm, 3
            VkFormat.B8g8r8Snorm, 3
            VkFormat.B8g8r8Uscaled, 3
            VkFormat.B8g8r8Sscaled, 3
            VkFormat.B8g8r8Uint, 3
            VkFormat.B8g8r8Sint, 3
            VkFormat.B8g8r8Srgb, 3
            VkFormat.R8g8b8a8Unorm, 4
            VkFormat.R8g8b8a8Snorm, 4
            VkFormat.R8g8b8a8Uscaled, 4
            VkFormat.R8g8b8a8Sscaled, 4
            VkFormat.R8g8b8a8Uint, 4
            VkFormat.R8g8b8a8Sint, 4
            VkFormat.R8g8b8a8Srgb, 4
            VkFormat.B8g8r8a8Unorm, 4
            VkFormat.B8g8r8a8Snorm, 4
            VkFormat.B8g8r8a8Uscaled, 4
            VkFormat.B8g8r8a8Sscaled, 4
            VkFormat.B8g8r8a8Uint, 4
            VkFormat.B8g8r8a8Sint, 4
            VkFormat.B8g8r8a8Srgb, 4
            VkFormat.A8b8g8r8UnormPack32, 4
            VkFormat.A8b8g8r8SnormPack32, 4
            VkFormat.A8b8g8r8UscaledPack32, 4
            VkFormat.A8b8g8r8SscaledPack32, 4
            VkFormat.A8b8g8r8UintPack32, 4
            VkFormat.A8b8g8r8SintPack32, 4
            VkFormat.A8b8g8r8SrgbPack32, 4
            VkFormat.A2r10g10b10UnormPack32, 4
            VkFormat.A2r10g10b10SnormPack32, 4
            VkFormat.A2r10g10b10UscaledPack32, 4
            VkFormat.A2r10g10b10SscaledPack32, 4
            VkFormat.A2r10g10b10UintPack32, 4
            VkFormat.A2r10g10b10SintPack32, 4
            VkFormat.A2b10g10r10UnormPack32, 4
            VkFormat.A2b10g10r10SnormPack32, 4
            VkFormat.A2b10g10r10UscaledPack32, 4
            VkFormat.A2b10g10r10SscaledPack32, 4
            VkFormat.A2b10g10r10UintPack32, 4
            VkFormat.A2b10g10r10SintPack32, 4
            VkFormat.R16Unorm, 2
            VkFormat.R16Snorm, 2
            VkFormat.R16Uscaled, 2
            VkFormat.R16Sscaled, 2
            VkFormat.R16Uint, 2
            VkFormat.R16Sint, 2
            VkFormat.R16Sfloat, 2
            VkFormat.R16g16Unorm, 4
            VkFormat.R16g16Snorm, 4
            VkFormat.R16g16Uscaled, 4
            VkFormat.R16g16Sscaled, 4
            VkFormat.R16g16Uint, 4
            VkFormat.R16g16Sint, 4
            VkFormat.R16g16Sfloat, 4
            VkFormat.R16g16b16Unorm, 6
            VkFormat.R16g16b16Snorm, 6
            VkFormat.R16g16b16Uscaled, 6
            VkFormat.R16g16b16Sscaled, 6
            VkFormat.R16g16b16Uint, 6
            VkFormat.R16g16b16Sint, 6
            VkFormat.R16g16b16Sfloat, 6
            VkFormat.R16g16b16a16Unorm, 8
            VkFormat.R16g16b16a16Snorm, 8
            VkFormat.R16g16b16a16Uscaled, 8
            VkFormat.R16g16b16a16Sscaled, 8
            VkFormat.R16g16b16a16Uint, 8
            VkFormat.R16g16b16a16Sint, 8
            VkFormat.R16g16b16a16Sfloat, 8
            VkFormat.R32Uint, 4
            VkFormat.R32Sint, 4
            VkFormat.R32Sfloat, 4
            VkFormat.R32g32Uint, 8
            VkFormat.R32g32Sint, 8
            VkFormat.R32g32Sfloat, 8
            VkFormat.R32g32b32Uint, 12
            VkFormat.R32g32b32Sint, 12
            VkFormat.R32g32b32Sfloat, 12
            VkFormat.R32g32b32a32Uint, 16
            VkFormat.R32g32b32a32Sint, 16
            VkFormat.R32g32b32a32Sfloat, 16
            VkFormat.R64Uint, 8
            VkFormat.R64Sint, 8
            VkFormat.R64Sfloat, 8
            VkFormat.R64g64Uint, 16
            VkFormat.R64g64Sint, 16
            VkFormat.R64g64Sfloat, 16
            VkFormat.R64g64b64Uint, 24
            VkFormat.R64g64b64Sint, 24
            VkFormat.R64g64b64Sfloat, 24
            VkFormat.R64g64b64a64Uint, 32
            VkFormat.R64g64b64a64Sint, 32
            VkFormat.R64g64b64a64Sfloat, 32
            VkFormat.B10g11r11UfloatPack32, 4
            VkFormat.E5b9g9r9UfloatPack32, 4
            VkFormat.D16Unorm, 2
            VkFormat.X8D24UnormPack32, 4
            VkFormat.D32Sfloat, 4
            VkFormat.S8Uint, 1
            VkFormat.D16UnormS8Uint, 3
            VkFormat.D24UnormS8Uint, 4
            VkFormat.D32SfloatS8Uint, 5
        ]

    let expectedType =
        LookupTable.lookup [
            VkFormat.Undefined, null
            VkFormat.R4g4UnormPack8, typeof<uint8>
            VkFormat.R4g4b4a4UnormPack16, typeof<uint16>
            VkFormat.B4g4r4a4UnormPack16, typeof<uint16>
            VkFormat.R5g6b5UnormPack16, typeof<uint16>
            VkFormat.B5g6r5UnormPack16, typeof<uint16>
            VkFormat.R5g5b5a1UnormPack16, typeof<uint16>
            VkFormat.B5g5r5a1UnormPack16, typeof<uint16>
            VkFormat.A1r5g5b5UnormPack16, typeof<uint16>
            VkFormat.R8Unorm, typeof<uint8>
            VkFormat.R8Snorm, typeof<int8>
            VkFormat.R8Uscaled, typeof<uint8>
            VkFormat.R8Sscaled, typeof<int8>
            VkFormat.R8Uint, typeof<uint8>
            VkFormat.R8Sint, typeof<int8>
            VkFormat.R8Srgb, typeof<uint8>
            VkFormat.R8g8Unorm, typeof<uint8>
            VkFormat.R8g8Snorm, typeof<int8>
            VkFormat.R8g8Uscaled, typeof<uint8>
            VkFormat.R8g8Sscaled, typeof<int8>
            VkFormat.R8g8Uint, typeof<uint8>
            VkFormat.R8g8Sint, typeof<int8>
            VkFormat.R8g8Srgb, typeof<uint8>
            VkFormat.R8g8b8Unorm, typeof<uint8>
            VkFormat.R8g8b8Snorm, typeof<int8>
            VkFormat.R8g8b8Uscaled, typeof<uint8>
            VkFormat.R8g8b8Sscaled, typeof<int8>
            VkFormat.R8g8b8Uint, typeof<uint8>
            VkFormat.R8g8b8Sint, typeof<int8>
            VkFormat.R8g8b8Srgb, typeof<uint8>
            VkFormat.B8g8r8Unorm, typeof<uint8>
            VkFormat.B8g8r8Snorm, typeof<int8>
            VkFormat.B8g8r8Uscaled, typeof<uint8>
            VkFormat.B8g8r8Sscaled, typeof<int8>
            VkFormat.B8g8r8Uint, typeof<uint8>
            VkFormat.B8g8r8Sint, typeof<int8>
            VkFormat.B8g8r8Srgb, typeof<uint8>
            VkFormat.R8g8b8a8Unorm, typeof<uint8>
            VkFormat.R8g8b8a8Snorm, typeof<int8>
            VkFormat.R8g8b8a8Uscaled, typeof<uint8>
            VkFormat.R8g8b8a8Sscaled, typeof<int8>
            VkFormat.R8g8b8a8Uint, typeof<uint8>
            VkFormat.R8g8b8a8Sint, typeof<int8>
            VkFormat.R8g8b8a8Srgb, typeof<uint8>
            VkFormat.B8g8r8a8Unorm, typeof<uint8>
            VkFormat.B8g8r8a8Snorm, typeof<int8>
            VkFormat.B8g8r8a8Uscaled, typeof<uint8>
            VkFormat.B8g8r8a8Sscaled, typeof<int8>
            VkFormat.B8g8r8a8Uint, typeof<uint8>
            VkFormat.B8g8r8a8Sint, typeof<int8>
            VkFormat.B8g8r8a8Srgb, typeof<uint8>
            VkFormat.A8b8g8r8UnormPack32, typeof<uint32>
            VkFormat.A8b8g8r8SnormPack32, typeof<uint32>
            VkFormat.A8b8g8r8UscaledPack32, typeof<uint32>
            VkFormat.A8b8g8r8SscaledPack32, typeof<uint32>
            VkFormat.A8b8g8r8UintPack32, typeof<uint32>
            VkFormat.A8b8g8r8SintPack32, typeof<uint32>
            VkFormat.A8b8g8r8SrgbPack32, typeof<uint32>
            VkFormat.A2r10g10b10UnormPack32, typeof<uint32>
            VkFormat.A2r10g10b10SnormPack32, typeof<uint32>
            VkFormat.A2r10g10b10UscaledPack32, typeof<uint32>
            VkFormat.A2r10g10b10SscaledPack32, typeof<uint32>
            VkFormat.A2r10g10b10UintPack32, typeof<uint32>
            VkFormat.A2r10g10b10SintPack32, typeof<uint32>
            VkFormat.A2b10g10r10UnormPack32, typeof<uint32>
            VkFormat.A2b10g10r10SnormPack32, typeof<uint32>
            VkFormat.A2b10g10r10UscaledPack32, typeof<uint32>
            VkFormat.A2b10g10r10SscaledPack32, typeof<uint32>
            VkFormat.A2b10g10r10UintPack32, typeof<uint32>
            VkFormat.A2b10g10r10SintPack32, typeof<uint32>
            VkFormat.R16Unorm, typeof<uint16>
            VkFormat.R16Snorm, typeof<int16>
            VkFormat.R16Uscaled, typeof<uint16>
            VkFormat.R16Sscaled, typeof<int16>
            VkFormat.R16Uint, typeof<uint16>
            VkFormat.R16Sint, typeof<int16>
            VkFormat.R16Sfloat, typeof<float16>
            VkFormat.R16g16Unorm, typeof<uint16>
            VkFormat.R16g16Snorm, typeof<int16>
            VkFormat.R16g16Uscaled, typeof<uint16>
            VkFormat.R16g16Sscaled, typeof<int16>
            VkFormat.R16g16Uint, typeof<uint16>
            VkFormat.R16g16Sint, typeof<int16>
            VkFormat.R16g16Sfloat, typeof<float16>
            VkFormat.R16g16b16Unorm, typeof<uint16>
            VkFormat.R16g16b16Snorm, typeof<int16>
            VkFormat.R16g16b16Uscaled, typeof<uint16>
            VkFormat.R16g16b16Sscaled, typeof<int16>
            VkFormat.R16g16b16Uint, typeof<uint16>
            VkFormat.R16g16b16Sint, typeof<int16>
            VkFormat.R16g16b16Sfloat, typeof<float16>
            VkFormat.R16g16b16a16Unorm, typeof<uint16>
            VkFormat.R16g16b16a16Snorm, typeof<int16>
            VkFormat.R16g16b16a16Uscaled, typeof<uint16>
            VkFormat.R16g16b16a16Sscaled, typeof<int16>
            VkFormat.R16g16b16a16Uint, typeof<uint16>
            VkFormat.R16g16b16a16Sint, typeof<int16>
            VkFormat.R16g16b16a16Sfloat, typeof<float16>
            VkFormat.R32Uint, typeof<uint32>
            VkFormat.R32Sint, typeof<int32>
            VkFormat.R32Sfloat, typeof<float32>
            VkFormat.R32g32Uint, typeof<uint32>
            VkFormat.R32g32Sint, typeof<int32>
            VkFormat.R32g32Sfloat, typeof<float32>
            VkFormat.R32g32b32Uint, typeof<uint32>
            VkFormat.R32g32b32Sint, typeof<int32>
            VkFormat.R32g32b32Sfloat, typeof<float32>
            VkFormat.R32g32b32a32Uint, typeof<uint32>
            VkFormat.R32g32b32a32Sint, typeof<int32>
            VkFormat.R32g32b32a32Sfloat, typeof<float32>
            VkFormat.R64Uint, typeof<uint64>
            VkFormat.R64Sint, typeof<int64>
            VkFormat.R64Sfloat, typeof<float>
            VkFormat.R64g64Uint, typeof<uint64>
            VkFormat.R64g64Sint, typeof<int64>
            VkFormat.R64g64Sfloat, typeof<float>
            VkFormat.R64g64b64Uint, typeof<uint64>
            VkFormat.R64g64b64Sint, typeof<int64>
            VkFormat.R64g64b64Sfloat, typeof<float>
            VkFormat.R64g64b64a64Uint, typeof<uint64>
            VkFormat.R64g64b64a64Sint, typeof<int64>
            VkFormat.R64g64b64a64Sfloat, typeof<float>
            VkFormat.B10g11r11UfloatPack32, typeof<uint32>
            VkFormat.E5b9g9r9UfloatPack32, typeof<uint32>
            VkFormat.D16Unorm, typeof<uint16>
            VkFormat.X8D24UnormPack32, typeof<uint32>
            VkFormat.D32Sfloat, typeof<float32>
            VkFormat.S8Uint, typeof<uint8>
            VkFormat.D16UnormS8Uint, typeof<uint32>
            VkFormat.D24UnormS8Uint, typeof<uint32>
            VkFormat.D32SfloatS8Uint, null
            VkFormat.Bc1RgbUnormBlock, null
            VkFormat.Bc1RgbSrgbBlock, null
            VkFormat.Bc1RgbaUnormBlock, null
            VkFormat.Bc1RgbaSrgbBlock, null
            VkFormat.Bc2UnormBlock, null
            VkFormat.Bc2SrgbBlock, null
            VkFormat.Bc3UnormBlock, null
            VkFormat.Bc3SrgbBlock, null
            VkFormat.Bc4UnormBlock, null
            VkFormat.Bc4SnormBlock, null
            VkFormat.Bc5UnormBlock, null
            VkFormat.Bc5SnormBlock, null
            VkFormat.Bc6hUfloatBlock, null
            VkFormat.Bc6hSfloatBlock, null
            VkFormat.Bc7UnormBlock, null
            VkFormat.Bc7SrgbBlock, null
            VkFormat.Etc2R8g8b8UnormBlock, null
            VkFormat.Etc2R8g8b8SrgbBlock, null
            VkFormat.Etc2R8g8b8a1UnormBlock, null
            VkFormat.Etc2R8g8b8a1SrgbBlock, null
            VkFormat.Etc2R8g8b8a8UnormBlock, null
            VkFormat.Etc2R8g8b8a8SrgbBlock, null
            VkFormat.EacR11UnormBlock, null
            VkFormat.EacR11SnormBlock, null
            VkFormat.EacR11g11UnormBlock, null
            VkFormat.EacR11g11SnormBlock, null
            VkFormat.Astc44UnormBlock, null
            VkFormat.Astc44SrgbBlock, null
            VkFormat.Astc54UnormBlock, null
            VkFormat.Astc54SrgbBlock, null
            VkFormat.Astc55UnormBlock, null
            VkFormat.Astc55SrgbBlock, null
            VkFormat.Astc65UnormBlock, null
            VkFormat.Astc65SrgbBlock, null
            VkFormat.Astc66UnormBlock, null
            VkFormat.Astc66SrgbBlock, null
            VkFormat.Astc85UnormBlock, null
            VkFormat.Astc85SrgbBlock, null
            VkFormat.Astc86UnormBlock, null
            VkFormat.Astc86SrgbBlock, null
            VkFormat.Astc88UnormBlock, null
            VkFormat.Astc88SrgbBlock, null
            VkFormat.Astc105UnormBlock, null
            VkFormat.Astc105SrgbBlock, null
            VkFormat.Astc106UnormBlock, null
            VkFormat.Astc106SrgbBlock, null
            VkFormat.Astc108UnormBlock, null
            VkFormat.Astc108SrgbBlock, null
            VkFormat.Astc1010UnormBlock, null
            VkFormat.Astc1010SrgbBlock, null
            VkFormat.Astc1210UnormBlock, null
            VkFormat.Astc1210SrgbBlock, null
            VkFormat.Astc1212UnormBlock, null
            VkFormat.Astc1212SrgbBlock, null
        ]

    let ofPixFormat (fmt : PixFormat) (t : TextureParams) =
        TextureFormat.ofPixFormat fmt t |> ofTextureFormat

    let private tryOfBaseType =
        let same value _arg = value
        let bgr x y flag = if flag then x else y

        let lookup =
            LookupTable.tryLookup [
                (typeof<float16>, 1), same VkFormat.R16Sfloat
                (typeof<float16>, 2), same VkFormat.R16g16Sfloat
                (typeof<float16>, 3), same VkFormat.R16g16b16Sfloat
                (typeof<float16>, 4), same VkFormat.R16g16b16a16Sfloat

                (typeof<float32>, 1), same VkFormat.R32Sfloat
                (typeof<float32>, 2), same VkFormat.R32g32Sfloat
                (typeof<float32>, 3), same VkFormat.R32g32b32Sfloat
                (typeof<float32>, 4), same VkFormat.R32g32b32a32Sfloat

                (typeof<float>, 1),   same VkFormat.R64Sfloat
                (typeof<float>, 2),   same VkFormat.R64g64Sfloat
                (typeof<float>, 3),   same VkFormat.R64g64b64Sfloat
                (typeof<float>, 4),   same VkFormat.R64g64b64a64Sfloat

                (typeof<int8>, 1),    same VkFormat.R8Sint
                (typeof<int8>, 2),    same VkFormat.R8g8Sint
                (typeof<int8>, 3),    same VkFormat.R8g8b8Sint
                (typeof<int8>, 4),    same VkFormat.R8g8b8a8Sint

                (typeof<uint8>, 1),   same VkFormat.R8Uint
                (typeof<uint8>, 2),   same VkFormat.R8g8Uint
                (typeof<uint8>, 3),   bgr  VkFormat.B8g8r8Uint VkFormat.R8g8b8Uint      // C3b and C4b have BGR layout
                (typeof<uint8>, 4),   bgr  VkFormat.B8g8r8a8Uint VkFormat.R8g8b8a8Uint

                (typeof<int16>, 1),   same VkFormat.R16Sint
                (typeof<int16>, 2),   same VkFormat.R16g16Sint
                (typeof<int16>, 3),   same VkFormat.R16g16b16Sint
                (typeof<int16>, 4),   same VkFormat.R16g16b16a16Sint

                (typeof<uint16>, 1),  same VkFormat.R16Uint
                (typeof<uint16>, 2),  same VkFormat.R16g16Uint
                (typeof<uint16>, 3),  same VkFormat.R16g16b16Uint
                (typeof<uint16>, 4),  same VkFormat.R16g16b16a16Uint

                (typeof<int32>, 1),   same VkFormat.R32Sint
                (typeof<int32>, 2),   same VkFormat.R32g32Sint
                (typeof<int32>, 3),   same VkFormat.R32g32b32Sint
                (typeof<int32>, 4),   same VkFormat.R32g32b32a32Sint

                (typeof<uint32>, 1),  same VkFormat.R32Uint
                (typeof<uint32>, 2),  same VkFormat.R32g32Uint
                (typeof<uint32>, 3),  same VkFormat.R32g32b32Uint
                (typeof<uint32>, 4),  same VkFormat.R32g32b32a32Uint
            ]

        let normLookup =
            LookupTable.tryLookup [
                VkFormat.R8Sint,           VkFormat.R8Snorm
                VkFormat.R8g8Sint,         VkFormat.R8g8Snorm
                VkFormat.R8g8b8Sint,       VkFormat.R8g8b8Snorm
                VkFormat.R8g8b8a8Sint,     VkFormat.R8g8b8a8Snorm

                VkFormat.R8Uint,           VkFormat.R8Unorm
                VkFormat.R8g8Uint,         VkFormat.R8g8Unorm
                VkFormat.R8g8b8Uint,       VkFormat.R8g8b8Unorm
                VkFormat.B8g8r8Uint,       VkFormat.B8g8r8Unorm
                VkFormat.R8g8b8a8Uint,     VkFormat.R8g8b8a8Unorm
                VkFormat.B8g8r8a8Uint,     VkFormat.B8g8r8a8Unorm

                VkFormat.R16Sint,          VkFormat.R16Snorm
                VkFormat.R16g16Sint,       VkFormat.R16g16Snorm
                VkFormat.R16g16b16Sint,    VkFormat.R16g16b16Snorm
                VkFormat.R16g16b16a16Sint, VkFormat.R16g16b16a16Snorm

                VkFormat.R16Uint,          VkFormat.R16Unorm
                VkFormat.R16g16Uint,       VkFormat.R16g16Unorm
                VkFormat.R16g16b16Uint,    VkFormat.R16g16b16Unorm
                VkFormat.R16g16b16a16Uint, VkFormat.R16g16b16a16Unorm
            ]

        let scaledLookup =
            LookupTable.tryLookup [
                VkFormat.R8Sint,           VkFormat.R8Sscaled
                VkFormat.R8g8Sint,         VkFormat.R8g8Sscaled
                VkFormat.R8g8b8Sint,       VkFormat.R8g8b8Sscaled
                VkFormat.R8g8b8a8Sint,     VkFormat.R8g8b8a8Sscaled

                VkFormat.R8Uint,           VkFormat.R8Uscaled
                VkFormat.R8g8Uint,         VkFormat.R8g8Uscaled
                VkFormat.R8g8b8Uint,       VkFormat.R8g8b8Uscaled
                VkFormat.B8g8r8Uint,       VkFormat.B8g8r8Uscaled
                VkFormat.R8g8b8a8Uint,     VkFormat.R8g8b8a8Uscaled
                VkFormat.B8g8r8a8Uint,     VkFormat.B8g8r8a8Uscaled

                VkFormat.R16Sint,          VkFormat.R16Sscaled
                VkFormat.R16g16Sint,       VkFormat.R16g16Sscaled
                VkFormat.R16g16b16Sint,    VkFormat.R16g16b16Sscaled
                VkFormat.R16g16b16a16Sint, VkFormat.R16g16b16a16Sscaled

                VkFormat.R16Uint,          VkFormat.R16Uscaled
                VkFormat.R16g16Uint,       VkFormat.R16g16Uscaled
                VkFormat.R16g16b16Uint,    VkFormat.R16g16b16Uscaled
                VkFormat.R16g16b16a16Uint, VkFormat.R16g16b16a16Uscaled
            ]

        fun (bgr : bool) (normalized : bool option) (dimension : int) (baseType : Type) ->
            lookup (baseType, dimension)
            |> Option.bind (fun f ->
                let rep =
                    match normalized with
                    | Some true -> normLookup
                    | Some false -> scaledLookup
                    | _ -> Some

                f bgr |> rep
            )

    let private (|Attribute|_|) (t : Type) =
        match t with
        | ColorOf(d, t) | VectorOf(d, t) -> Some (d, t)
        | MatrixOf(s, t) -> Some (s.X, t)
        | Numeric -> Some (1, t)
        | _ -> None

    let inline private isBgr (t : Type) =
        match t with
        | ColorOf(_, UInt8) -> true
        | _ -> false

    let tryGetAttributeFormat (expectedType : Type) (inputType : Type) (normalized : bool) =
        match inputType, expectedType with
        | Attribute(dim, (Integral as bt)), Attribute(_, (Float32 | Float64)) ->
            bt |> tryOfBaseType (isBgr inputType) (Some normalized) dim

        | Attribute(dim, btIn), Attribute(_, btEx) when btIn = btEx ->
            btEx |> tryOfBaseType (isBgr inputType) None dim

        | _ ->
            None