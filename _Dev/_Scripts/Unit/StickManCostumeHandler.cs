using UnityEngine;

namespace Game.Unit
{
    public class StickManCostumeHandler : MonoBehaviour
    {
        private StickManData[] _stickManDatas;

        #region PUBLIC METHODS

        public void Init(StickManData[] stickManDatas)
        {
            _stickManDatas = stickManDatas;
        }

        public virtual void SetCostumes(int currentYear)
        {
            ResetCostumes();

            // Return early if year maxed out
            if (currentYear >= _stickManDatas[^1].YearsToUpgrade)
            {
                foreach (var costume in _stickManDatas[^1].Costumes)
                    costume.SetActive(true);
                return;
            }


            for (int i = 0; i < _stickManDatas.Length; i++)
            {
                if (currentYear < _stickManDatas[i].YearsToUpgrade)
                {
                    foreach (var costume in _stickManDatas[i - 1].Costumes)
                        costume.SetActive(true);

                    break;
                }
            }
        }

        #endregion

        #region PRIVATE METHODS

        private void ResetCostumes()
        {
            foreach (var data in _stickManDatas)
            foreach (var costume in data.Costumes)
                costume.SetActive(false);
        }

        #endregion
    }
}