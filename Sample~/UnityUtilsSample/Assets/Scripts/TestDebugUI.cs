using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api;
using Api.Data;
using DebugWidget.UI.Runtime;
using Http.ManagerExt.Runtime;
using UnityEngine;

namespace Sample
{
    public class TestDebugUI : DebugUI
    {
        protected override void Start()
        {
            base.Start();
            ControlPanelUI(Kv.Debug.GetBool("show_debug_ui", true));
        }

        protected override void InitStaticItems()
        {
            base.InitStaticItems();
            ListRectTf.CreateButton($"Test Loading", async (btn, text) =>
            {
                ControlLoading(true);
                await Task.Delay(TimeSpan.FromSeconds(5));
                ControlLoading(false);
            });
            ListRectTf.CreateButton($"Test HttpManager", async (btn, text) =>
            {
                ControlLoading(true);

                var result = await GithubApi.Instance.contributors("square", "retrofit");
                var list = result.SuccessOr();
                Debug.Log($"HttpManager, GithubApi.contributors.list={list?.Count}");

                ControlLoading(false);
            });
        }
    }
}