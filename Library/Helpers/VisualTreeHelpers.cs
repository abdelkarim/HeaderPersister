// Copyright (c) 'Abdelkarim Sellamna'. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Media;

namespace WpfGhost.Controls.Helpers
{
    public static class VisualTreeHelpers
    {
        public static T FindParent<T>(this FrameworkElement child) where T : DependencyObject
        {
            T parent = null;
            var currentParent = VisualTreeHelper.GetParent(child);

            while (currentParent != null)
            {

                // check the current parent
                if (currentParent is T)
                {
                    parent = (T) currentParent;
                    break;
                }

                // find the next parent
                currentParent = VisualTreeHelper.GetParent(currentParent);
            }

            return parent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static T FindChild<T>(this FrameworkElement parent) where T : DependencyObject
        {
            T child = null;

            var count = VisualTreeHelper.GetChildrenCount(parent);
            if (count == 0)
            {
                return null;
            }

            // check the children
            for (var i = 0; i < count; i++)
            {
                var currentChild = VisualTreeHelper.GetChild(parent, i);
                if (currentChild is T)
                {
                    child = (T) currentChild;
                    break;
                }

                // iterate over child sub-tree if nothing is yet found
                currentChild = FindChild<T>((FrameworkElement) currentChild);
                if (currentChild != null)
                {
                    child = (T) currentChild;
                    break;
                }
            }

            return child;
        }
    }
}
