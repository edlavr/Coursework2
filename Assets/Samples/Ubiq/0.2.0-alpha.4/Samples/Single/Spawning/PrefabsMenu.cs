﻿using System.Collections.Generic;
using System.Linq;
using Samples.Ubiq._0._2._0_alpha._4.Samples.Intro.Scripts;
using UnityEngine;

namespace Samples.Ubiq._0._2._0_alpha._4.Samples.Single.Spawning
{
    public class PrefabsMenu : MonoBehaviour
    {
        public PrefabCatalogue Catalogue;

        private List<SpawnPrefabControl> Controls;

        private void Awake()
        {
            Controls = GetComponentsInChildren<SpawnPrefabControl>(true).ToList();
        }

        // Start is called before the first frame update
        void Start()
        {
            UpdateCatalogue();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateCatalogue()
        {
            while(Controls.Count < Catalogue.prefabs.Count)
            {
                var Control = GameObject.Instantiate(Controls.First().gameObject, transform);
                Controls.Add(Control.GetComponent<SpawnPrefabControl>());
            }

            for (int i = 0; i < Controls.Count; i++)
            {
                Controls[i].gameObject.SetActive(i < Catalogue.prefabs.Count);
            }

            for (int i = 0; i < Catalogue.prefabs.Count; i++)
            {
                Controls[i].SetPrefab(Catalogue.prefabs[i]);
            }
        }
    }
}