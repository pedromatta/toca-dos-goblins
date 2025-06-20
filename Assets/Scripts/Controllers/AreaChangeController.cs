using System.Collections;
using UnityEngine;
using Assets.Scripts.Managers;
using Assets.Scripts.World;

namespace Assets.Scripts.Controllers
{
    public class AreaChangeController : MonoBehaviour
    {
        public GameObject TeleporterNext;
        public GameObject TeleporterPrevious;

        private Collider2D _teleporterNextCollider;
        private Collider2D _teleporterPreviousCollider;

        private void Start()
        {
            if (TeleporterNext != null)
                _teleporterNextCollider = TeleporterNext.GetComponent<Collider2D>();

            if (TeleporterPrevious != null)
                _teleporterPreviousCollider = TeleporterPrevious.GetComponent<Collider2D>();
        }

        public void NextArea()
        {
            var mundo = GameManager.Instance.Mundo;
            var currentArea = GameManager.Instance.CurrentArea;
            if (mundo != null && currentArea != null)
            {
                int idx = mundo.Areas.IndexOf(currentArea);
                if (idx < mundo.Areas.Count - 1)
                {
                    var nextArea = mundo.Areas[idx + 1];
                    GameManager.Instance.ChangeArea(nextArea.Nome);
                }
            }
        }

        public void PreviousArea()
        {
            var mundo = GameManager.Instance.Mundo;
            var currentArea = GameManager.Instance.CurrentArea;
            if (mundo != null && currentArea != null)
            {
                int idx = mundo.Areas.IndexOf(currentArea);
                if (idx > 0)
                {
                    var nextArea = mundo.Areas[idx - 1];
                    GameManager.Instance.ChangeArea(nextArea.Nome);
                }
            }
        }
    }
}