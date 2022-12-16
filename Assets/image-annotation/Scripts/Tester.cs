using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ImageAnnotation
{
    public class Tester : MonoBehaviour
    {
        public RawImage imagePrefab;
        void Start()
        {
            StartCoroutine(LoadImages());
        }

        void Update()
        {
        
        }

        IEnumerator LoadImages()
        {
            var getPuzzle = ImageLoader.GetPuzzle();
            yield return new WaitUntil(() => getPuzzle.isDone);
            var res = getPuzzle.GetResult();
            foreach(var puzzle in res.puzzles)
            {
                var loadImage = ImageLoader.LoadImage("localhost:3000/slices/" + puzzle.image);
                yield return new WaitUntil(() => loadImage.isDone);
                var tex2d = loadImage.GetResult();
                var child = Instantiate(imagePrefab, transform);
                child.texture = tex2d;
                yield return new WaitForSeconds(Random.Range(0.2f,0.9f));
            }
        }
    }
}
