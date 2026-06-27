using Dalamud.Interface.Textures.TextureWraps;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Text;


//Required code lifted from KamiToolKit

namespace WrathCombo.Extensions
{
    internal unsafe static class AtkExtensions
    {
        extension(ref AtkUldPart part)
        {
            /// <summary>
            /// Load a texture from a Texture* directly.
            /// </summary>
            /// <remarks>
            /// <em>Warning, calling this multiple times on the same part may corrupt the game state.</em>
            /// Additionally unloading this part with a Texture* set may attempt to release the texture that wasn't owned in the first place.
            /// Undefined behavior may result.
            /// </remarks>
            /// <param name="texture">Texture to load.</param>
            public void LoadTexture(Texture* texture)
            {
                if (part.UldAsset is null) return;

                part.TryUnloadTexture();
                part.UldAsset->AtkTexture.KernelTexture = texture;
                part.UldAsset->AtkTexture.TextureType = TextureType.KernelTexture;
            }

            /// <summary>
            /// Loads texture from a IDalamudTextureWrap.
            /// </summary>
            /// <remarks>
            /// The texture wrap must remain valid for the lifetime of this node.
            /// </remarks>
            /// <param name="textureWrap">Texture wrap to load.</param>
            public void LoadTexture(IDalamudTextureWrap textureWrap)
            {
                var texturePointer = (Texture*)Svc.Texture.ConvertToKernelTexture(textureWrap, true);
                if (texturePointer is null) return;

                part.LoadTexture(texturePointer);
            }

            private void TryUnloadTexture()
            {
                if (part.UldAsset is null) return;
                if (!part.UldAsset->AtkTexture.IsTextureReady()) return;
                if (part.UldAsset->AtkTexture.TextureType is 0) return;
                if (part.UldAsset->AtkTexture.KernelTexture is null) return;

                part.UldAsset->AtkTexture.ReleaseTexture();
                part.UldAsset->AtkTexture.KernelTexture = null;
                part.UldAsset->AtkTexture.TextureType = 0;
            }
        }
    }
}
