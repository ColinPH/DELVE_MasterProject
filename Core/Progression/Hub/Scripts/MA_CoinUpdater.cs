using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class MA_CoinUpdater : LocalManagerAction
    {
        [SerializeField] List<GameObject> coinPiles = null;
        private int coinIndex = 0;

        protected override void MyStart()
        {
            base.MyStart();


        }

        public override void InvokeLocalManagerAction(LocalManager localManager)
        {
            base.InvokeLocalManagerAction(localManager);
            int coins = Metrics.player.collectedCoinsAmount;

            switch (coins)
            {
                case >= 1000:
                    DeactivateCurrent();
                    coinIndex = 5;
                    ActivateNew();
                    break;

                case >= 500:
                    DeactivateCurrent();
                    coinIndex = 4;
                    ActivateNew();

                    break;

                case >= 400:
                    DeactivateCurrent(); 
                    coinIndex = 3;
                    ActivateNew();

                    break;

                case >= 300:
                    DeactivateCurrent(); 
                    coinIndex =2;
                    ActivateNew();

                    break;

                case >= 200:
                    DeactivateCurrent(); 
                    coinIndex =1;
                    ActivateNew();

                    break;

                case >= 100:
                    DeactivateCurrent();
                    coinIndex=0;
                    coinPiles[coinIndex].SetActive(true);
                    break;


                default:
                    DeactivateCurrent();
                    coinIndex = 0;
                    break;
            }


        }

        private void ActivateNew()
        {
            coinPiles[coinIndex].SetActive(true);
        }

        private void DeactivateCurrent()
        {
            coinPiles[coinIndex].SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
