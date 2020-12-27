using System.Collections.Generic;
using System.Linq;

namespace DeXcor.Helpers
{
    public static class ImagesNavigationHelper
    {
        private static Dictionary<string, Stack<int>> _imageGalleriesHistories = new Dictionary<string, Stack<int>>();

        public static void AddImageId(string imageGalleryId, int imageId)
        {
            var stack = GetStack(imageGalleryId);
            stack.Push(imageId);
        }

        public static void UpdateImageId(string imageGalleryId, int imageId)
        {
            var stack = GetStack(imageGalleryId);
            if (stack.Any())
            {
                stack.Pop();
            }

            stack.Push(imageId);
        }

        public static int GetImageId(string imageGalleryId)
        {
            var stack = GetStack(imageGalleryId);
            return stack.Any() ? stack.Peek() : 0;
        }

        public static void RemoveImageId(string imageGalleryId)
        {
            var stack = GetStack(imageGalleryId);
            if (stack.Any())
            {
                stack.Pop();
            }
        }

        private static Stack<int> GetStack(string imageGalleryId)
        {
            if (!_imageGalleriesHistories.Keys.Contains(imageGalleryId))
            {
                _imageGalleriesHistories.Add(imageGalleryId, new Stack<int>());
            }
            return _imageGalleriesHistories[imageGalleryId];
        }
    }
}
