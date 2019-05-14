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
    /// <param name="loader">ロードするオブジェクト</param>
    /// <returns>タスク</returns>
    public delegate Task LoadFunc(ILoader loader);

    /// <summary>
    /// ローディングシーン
    /// </summary>
    public abstract class LoadingScene : asd.Scene,ILoader
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
        public float Progress => ProgressInfo.taskCount != 0 ? ProgressInfo.progress / (float)ProgressInfo.taskCount : 0;

        public (int taskCount, int progress) ProgressInfo { get; set; } = (0, 0);
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

            task = LoadFunc(this);
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
