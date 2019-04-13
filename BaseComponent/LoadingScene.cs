using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent
{
    /// <summary>
    /// ロード用デリゲート
    /// </summary>
    /// <param name="taskCount">タスク数</param>
    /// <param name="progress">完了したタスク数</param>
    /// <returns>タスク</returns>
    public delegate Task LoadFunc((int taskCount, int progress) info);

    /// <summary>
    /// ローディングシーン
    /// </summary>
    public abstract class LoadingScene : asd.Scene
    {
        /// <summary>
        /// 次に遷移するシーン
        /// </summary>
        public asd.Scene NextScene { get; }

        /// <summary>
        /// ロード用デリゲート
        /// </summary>
        public LoadFunc LoadFunc { get; }

        /// <summary>
        /// 進捗(0.0-1.0)
        /// </summary>
        public float Progress => info.taskCount != 0 ? info.progress / (float)info.taskCount : 0;

        (int taskCount ,int progress) info = (0, 0);
        Task task;
        private readonly asd.Transition transition;
        private IEnumerator<object> coroutine;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nextScene">次に遷移するシーン</param>
        /// <param name="loadFunc">ロード用デリゲート</param>
        public LoadingScene(asd.Scene nextScene, LoadFunc loadFunc, asd.Transition transition = null)
        {
            NextScene = nextScene;
            LoadFunc = loadFunc;
            this.transition = transition;
            coroutine = Update();
        }

        protected override void OnStartUpdating()
        {

            task = LoadFunc(info);
            base.OnStartUpdating();
        }

        protected override void OnUpdated()
        {
            coroutine?.MoveNext();
            base.OnUpdated();
        }

        IEnumerator<object> Update()
        {
            while (!task.IsCanceled && !task.IsCompleted && !task.IsFaulted)
            {
                yield return null;
            }

            if (task.IsCompleted)
            {
                if (transition == null)
                    asd.Engine.ChangeScene(NextScene);
                else
                    asd.Engine.ChangeSceneWithTransition(NextScene, transition);
            }
            if (task.IsCanceled || task.IsFaulted)
            {
                ErrorIO.AddError(new OperationCanceledException(NextScene.ToString() + "のロードに失敗しました."));
            }
        }
    }
}
