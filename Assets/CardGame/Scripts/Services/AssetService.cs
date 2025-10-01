using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace CardGame
{
    public class AssetsService : MonoBehaviour
    {
        [SerializeField, ReadOnly] private string jsonUrl = "https://drive.usercontent.google.com/download?id=1aYuc8oZ7tcYLC9v12AXPlWwvKLm_yXcE&export=download&authuser=1&confirm=t&uuid=2df6690c-becc-4399-9aa2-b573e0126b00&at=AN8xHormSPAxIOOnZo8f09hkUtaw:1759272588496";

        public List<Sprite> CardSprites { get; private set; }
        
        public void SetSprites(List<Sprite> sprites)
        {
            CardSprites = sprites;
        }
        
        public async UniTask LoadImagesAsync()
        {
            string json = await LoadJsonAsync();
            if (string.IsNullOrEmpty(json)) return;

            ImageData imageData = JsonConvert.DeserializeObject<ImageData>(json);
            if (imageData?.images == null || imageData.images.Count == 0)
            {
                Debug.LogError("No image data found");
                return;
            }

            var loadedSprites = await LoadAllImagesAsync(imageData.images);
            SetSprites(loadedSprites.Values.ToList());
        }
        
        
        private async UniTask<string> LoadJsonAsync()
        {
            using (UnityWebRequest request = UnityWebRequest.Get(jsonUrl))
            {
                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Failed to load JSON: " + request.error);
                    return null;
                }

                return request.downloadHandler.text;
            }
        }
        
        private async UniTask<Dictionary<string, Sprite>> LoadAllImagesAsync(List<ImageInfo> imageInfos)
        {
            Dictionary<string, Sprite> loadedSprites = new Dictionary<string, Sprite>();
            List<UniTask<(string name, Sprite sprite)>> tasks = new List<UniTask<(string, Sprite)>>();

            foreach (ImageInfo info in imageInfos)
            {
                tasks.Add(LoadImageAsync(info));
            }

            var results = await UniTask.WhenAll(tasks);

            foreach (var (name, sprite) in results)
            {
                if (!string.IsNullOrEmpty(name) && sprite != null && !loadedSprites.ContainsKey(name))
                {
                    loadedSprites.Add(name, sprite);
                }
            }

            return loadedSprites;
        }

        private async UniTask<(string, Sprite)> LoadImageAsync(ImageInfo info)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(info.url))
            {
                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to load image: {info.url}");
                    return (null, null);
                }

                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));
                return (info.name, sprite);
            }
        }
        
        
        [System.Serializable]
        private class ImageData
        {
            public List<ImageInfo> images;
        }

        [System.Serializable]
        private class ImageInfo
        {
            public string name;
            public string url;
        }
    }
}