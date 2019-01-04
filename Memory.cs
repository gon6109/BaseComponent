using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent
{
    /// <summary>
    /// メモリ開放系
    /// </summary>
    public static class Memory
    {
        public static void Release(Action action)
        {
            var layers = asd.Engine.CurrentScene.Layers;
            var objects = asd.Engine.CurrentScene.Layers.OfType<asd.Layer2D>().SelectMany(obj => obj.Objects).ToList();
            var scene = new Scene();
            asd.Engine.ChangeScene(scene);
            while (!scene.IsTrans)
            {
                if (!asd.Engine.DoEvents()) Environment.Exit(-1);
                action();

                CSharpScript.Create("", ScriptOptions.Default).RunAsync();
                asd.Engine.Update();
            }
            foreach (var item in layers)
            {
                item.ForceToRelease();
            }
            foreach (var item in objects)
            {
                var temp = (item as MultiAnimationObject2D)?.AnimationPart.SelectMany(obj => obj.Value.Textures) ?? new List<asd.Texture2D>();
                foreach (var item2 in temp)
                {
                    item2.ForceToRelease();
                }
                (item as asd.TextureObject2D)?.Texture?.ForceToRelease();
                item.ForceToRelease();
            }

            action();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            asd.Engine.Update();
        }

        public class Scene : asd.Scene
        {
            public bool isEndTrasition;
            public bool IsTrans { get; private set; }
            public int count = 10;
            protected override void OnStartUpdating()
            {
                isEndTrasition = true;
                base.OnStartUpdating();
            }

            protected override void OnUpdated()
            {
                if (count == 0)
                {
                    IsTrans = true;
                }
                if (isEndTrasition) count--;
                base.OnUpdated();
            }
        }
    }
}
