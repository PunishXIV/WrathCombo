using FFXIVClientStructs.FFXIV.Client.LayoutEngine;
using FFXIVClientStructs.Interop;
using FFXIVClientStructs.STD;
using System;
using System.Collections.Generic;
using System.Text;

namespace WrathCombo.Extensions
{
    internal unsafe static class LayoutExtensions
    {
        public static V* FindPtr<K, V>(ref this StdMap<K, Pointer<V>> map, K key) where K : unmanaged, IComparable where V : unmanaged
        {
            return map.TryGetValuePointer(key, out var ptr) && ptr != null ? ptr->Value : null;
        }

        public static ILayoutInstance* FindInstance(LayoutManager* layout, ulong key)
        {
            foreach (var (ikt, ikv) in layout->InstancesByType)
            {
                var iter = ikv.Value->FindPtr(key);
                if (iter != null)
                    return iter;
            }
            return null;
        }
    }
}
