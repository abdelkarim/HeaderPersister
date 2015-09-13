using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;

namespace WpfGhost.Controls.Helpers
{
    internal static class ItemContainerGeneratorExtensions
    {
        private static readonly MethodInfo CachedItemsGetterMethod;

        static ItemContainerGeneratorExtensions()
        {
            CachedItemsGetterMethod = typeof(ItemContainerGenerator).GetProperty("Items", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetGetMethod();

        }

        internal static IList<object> GetItems(this ItemContainerGenerator target)
        {
            if (CachedItemsGetterMethod == null)
            {
                throw new InvalidOperationException();
            }

            return CachedItemsGetterMethod.Invoke(target, null) as IList<object>;
        }
    }
}
