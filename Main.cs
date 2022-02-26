﻿using System.Collections.Generic;
using xiaoye97;
using HarmonyLib;
using BepInEx;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Reflection;
using CommonAPI;
using CommonAPI.Systems;
using ProjectGenesis.Patches;
using ERecipeType_1 = ERecipeType;

// ReSharper disable UnusedVariable
// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming

namespace ProjectGenesis
{
    [BepInPlugin("org.LoShin.GenesisBook", "Genesis", "1.0.0")]
    [BepInDependency("me.xiaoye97.plugin.Dyson.LDBTool")]
    [BepInDependency("dsp.common-api.CommonAPI")]
    [CommonAPISubmoduleDependency(nameof(ProtoRegistry), nameof(CustomDescSystem), nameof(TabSystem))]
    public class Main : BaseUnityPlugin
    {
        public void Awake()
        {
            var pluginfolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var resources = new ResourceData("org.LoShin.GenesisBook", "texpack", pluginfolder);
            resources.LoadAssetBundle("texpack");
            ProtoRegistry.AddResource(resources);

            int TableID = TabSystem.RegisterTab("org.LoShin.GenesisBook:org.LoShin.GenesisBookTab",
                                                new TabData("3", "Assets/texpack/主机科技"));
            int TableID2 = TabSystem.RegisterTab("org.LoShin.GenesisBook:org.LoShin.GenesisBookTab2",
                                                 new TabData("4", "Assets/texpack/化工科技"));

            LDBTool.PreAddDataAction += InitData;
            LDBTool.PostAddDataAction += PostAddDataAction;

            Harmony.CreateAndPatchAll(typeof(UIPatches));
            Harmony.CreateAndPatchAll(typeof(MultiProductionPatches));
            Harmony.CreateAndPatchAll(typeof(PlanetGasPatches));
            Harmony.CreateAndPatchAll(typeof(OceanDischargePatches));
        }

        /// <summary>
        /// 目前还是用不了
        /// </summary>
        public void InitData()
        {
            PreFix();

            // ref var techs = ref AccessTools.StaticFieldRefAccess<TechProtoSet>(typeof(LDB), "_techs");
            // ref var items = ref AccessTools.StaticFieldRefAccess<ItemProtoSet>(typeof(LDB), "_items");
            // ref var recipes = ref AccessTools.StaticFieldRefAccess<RecipeProtoSet>(typeof(LDB), "_recipes");
            // techs.Init(0);
            // items.Init(0);
            // recipes.Init(0);

            var ItemProtos = new Dictionary<int, ItemProto>();
            var RecipeProtos = new Dictionary<int, RecipeProto>();
            var TechProtos = new Dictionary<int, TechProto>();

            foreach (var techjson in JsonHelper.TechProtos())
            {
                var proto = LDB.techs.Exist(techjson.ID) ? LDB.techs.Select(techjson.ID) : null;

                if (proto == null)
                {
                    proto = new TechProto()
                    {
                        ID = techjson.ID,
                        Name = techjson.Name,
                        Desc = techjson.Desc,
                        Conclusion = techjson.Conclusion,
                        Published = techjson.Published,
                        Level = techjson.Level,
                        MaxLevel = techjson.MaxLevel,
                        LevelCoef1 = techjson.LevelCoef1,
                        LevelCoef2 = techjson.LevelCoef2,
                        IconPath = techjson.IconPath,
                        IsLabTech = techjson.IsLabTech,
                        PreTechs = techjson.PreTechs ?? new int[] { },
                        PreTechsImplicit = techjson.PreTechsImplicit ?? new int[] { },
                        PreTechsMax = techjson.PreTechsMax,
                        Items = techjson.Items ?? new int[] { },
                        ItemPoints = techjson.ItemPoints ?? new int[] { },
                        HashNeeded = techjson.HashNeeded,
                        UnlockRecipes = techjson.UnlockRecipes ?? new int[] { },
                        unlockRecipeArray = new RecipeProto[] { },
                        UnlockFunctions = techjson.UnlockFunctions ?? new int[] { },
                        UnlockValues = techjson.UnlockValues ?? new double[] { },
                        AddItems = techjson.AddItems ?? new int[] { },
                        AddItemCounts = techjson.AddItemCounts ?? new int[] { },
                        Position = new Vector2(techjson.Position[0], techjson.Position[1]),
                    };
                    LDBTool.PreAddProto(proto);
                } 
                else
                {
                    proto.ID = techjson.ID;
                    proto.Name = techjson.Name;
                    proto.Desc = techjson.Desc;
                    proto.Conclusion = techjson.Conclusion;
                    proto.Published = techjson.Published;
                    proto.Level = techjson.Level;
                    proto.MaxLevel = techjson.MaxLevel;
                    proto.LevelCoef1 = techjson.LevelCoef1;
                    proto.LevelCoef2 = techjson.LevelCoef2;
                    proto.IconPath = techjson.IconPath;
                    proto.IsLabTech = techjson.IsLabTech;
                    proto.PreTechs = techjson.PreTechs ?? new int[] { };
                    proto.PreTechsImplicit = techjson.PreTechsImplicit ?? new int[] { };
                    proto.PreTechsMax = techjson.PreTechsMax;
                    proto.Items = techjson.Items ?? new int[] { };
                    proto.ItemPoints = techjson.ItemPoints ?? new int[] { };
                    proto.HashNeeded = techjson.HashNeeded;
                    proto.UnlockRecipes = techjson.UnlockRecipes ?? new int[] { };
                    proto.unlockRecipeArray = new RecipeProto[] { };
                    proto.UnlockFunctions = techjson.UnlockFunctions ?? new int[] { };
                    proto.UnlockValues = techjson.UnlockValues ?? new double[] { };
                    proto.AddItems = techjson.AddItems ?? new int[] { };
                    proto.AddItemCounts = techjson.AddItemCounts ?? new int[] { };
                    proto.Position = new Vector2(techjson.Position[0], techjson.Position[1]);
                }
                TechProtos.Add(proto.ID, proto);
            }

            foreach (var itemjson in JsonHelper.ItemProtos())
            {
                var proto = LDB.items.Exist(itemjson.ID) ? LDB.items.Select(itemjson.ID) : null;
                
                if (proto == null)
                {
                    proto = new ItemProto()
                    {
                        ID = itemjson.ID,
                        Name = itemjson.Name,
                        Description = itemjson.Description,
                        IconPath = itemjson.IconPath,
                        GridIndex = itemjson.GridIndex,
                        StackSize = itemjson.StackSize,
                        preTech = TechProtos[itemjson.PreTech],
                        FuelType = itemjson.FuelType,
                        HeatValue = itemjson.HeatValue,
                        ReactorInc = itemjson.ReactorInc,
                        DescFields = itemjson.DescFields ?? new int[] { },
                        IsFluid = itemjson.IsFluid,
                        Type = (EItemType)itemjson.Type,
                        SubID = itemjson.SubID,
                        MiningFrom = itemjson.MiningFrom,
                        ProduceFrom = itemjson.ProduceFrom,
                        Grade = itemjson.Grade,
                        Upgrades = itemjson.Upgrades ?? new int[] { },
                        IsEntity = itemjson.IsEntity,
                        CanBuild = itemjson.CanBuild,
                        BuildInGas = itemjson.BuildInGas,
                        ModelIndex = itemjson.ModelIndex,
                        ModelCount = itemjson.ModelCount,
                        HpMax = itemjson.HpMax,
                        Ability = itemjson.Ability,
                        Potential = itemjson.Potential,
                        BuildIndex = itemjson.BuildIndex,
                        BuildMode = itemjson.BuildMode,
                        UnlockKey = itemjson.UnlockKey,
                        PreTechOverride = itemjson.PreTechOverride,
                        Productive = itemjson.Productive,
                        MechaMaterialID = itemjson.MechaMaterialID,
                        prefabDesc = PrefabDesc.none,
                    };
                    LDBTool.PreAddProto(proto);
                }
                else
                {
                    proto.ID = itemjson.ID;
                    proto.Name = itemjson.Name;
                    proto.Description = itemjson.Description;
                    proto.IconPath = itemjson.IconPath;
                    proto.GridIndex = itemjson.GridIndex;
                    proto.StackSize = itemjson.StackSize;
                    proto.preTech = TechProtos[itemjson.PreTech];
                    proto.FuelType = itemjson.FuelType;
                    proto.HeatValue = itemjson.HeatValue;
                    proto.ReactorInc = itemjson.ReactorInc;
                    proto.DescFields = itemjson.DescFields ?? new int[] { };
                    proto.IsFluid = itemjson.IsFluid;
                    proto.Type = (EItemType)itemjson.Type;
                    proto.SubID = itemjson.SubID;
                    proto.MiningFrom = itemjson.MiningFrom;
                    proto.ProduceFrom = itemjson.ProduceFrom;
                    proto.Grade = itemjson.Grade;
                    proto.Upgrades = itemjson.Upgrades ?? new int[] { };
                    proto.IsEntity = itemjson.IsEntity;
                    proto.CanBuild = itemjson.CanBuild;
                    proto.BuildInGas = itemjson.BuildInGas;
                    proto.ModelIndex = itemjson.ModelIndex;
                    proto.ModelCount = itemjson.ModelCount;
                    proto.HpMax = itemjson.HpMax;
                    proto.Ability = itemjson.Ability;
                    proto.Potential = itemjson.Potential;
                    proto.BuildIndex = itemjson.BuildIndex;
                    proto.BuildMode = itemjson.BuildMode;
                    proto.UnlockKey = itemjson.UnlockKey;
                    proto.PreTechOverride = itemjson.PreTechOverride;
                    proto.Productive = itemjson.Productive;
                    proto.MechaMaterialID = itemjson.MechaMaterialID;
                }
                ItemProtos.Add(proto.ID, proto); 
            }

            foreach (var recipeJson in JsonHelper.RecipeProtos())
            {
                var proto = LDB.recipes.Exist(recipeJson.ID) ? LDB.recipes.Select(recipeJson.ID) : null;
                if (proto == null)
                {
                    proto = new RecipeProto()
                    {
                        ID = recipeJson.ID,
                        Name = recipeJson.Name,
                        Description = recipeJson.Description,
                        IconPath = recipeJson.IconPath,
                        GridIndex = recipeJson.GridIndex,
                        Type = (ERecipeType_1)recipeJson.Type,
                        Handcraft = recipeJson.Handcraft,
                        TimeSpend = recipeJson.Time,
                        NonProductive = recipeJson.NonProductive,
                        preTech = TechProtos[recipeJson.PreTech],
                        Explicit = recipeJson.Explicit,
                        Items = recipeJson.Input ?? new int[] { },
                        ItemCounts = recipeJson.InCounts ?? new int[] { },
                        Results = recipeJson.Output ?? new int[] { },
                        ResultCounts = recipeJson.OutCounts ?? new int[] { }
                    };
                    LDBTool.PreAddProto(proto);
                }
                else
                {
                    proto.ID = recipeJson.ID;
                    proto.Name = recipeJson.Name;
                    proto.Description = recipeJson.Description;
                    proto.IconPath = recipeJson.IconPath;
                    proto.GridIndex = recipeJson.GridIndex;
                    proto.Type = (ERecipeType_1)recipeJson.Type;
                    proto.Handcraft = recipeJson.Handcraft;
                    proto.TimeSpend = recipeJson.Time;
                    proto.NonProductive = recipeJson.NonProductive;
                    proto.preTech = TechProtos[recipeJson.PreTech];
                    proto.Explicit = recipeJson.Explicit;
                    proto.Items = recipeJson.Input ?? new int[] { };
                    proto.ItemCounts = recipeJson.InCounts ?? new int[] { };
                    proto.Results = recipeJson.Output ?? new int[] { };
                    proto.ResultCounts = recipeJson.OutCounts ?? new int[] { };
                }
                RecipeProtos.Add(proto.ID, proto);
            }

            //techs.dataArray = new List<TechProto>(TechProtos.Values).ToArray();
            //items.dataArray = new List<ItemProto>(ItemProtos.Values).ToArray();
            //recipes.dataArray = new List<RecipeProto>(RecipeProtos.Values).ToArray();




            //var index = 0;
            // foreach (var item in ItemProtos.Values)
            // {
             //   item.prefabDesc = PrefabDesc.none;
                //item.Preload(index++);
                //LDBTool.PreAddProto(item);
            //}

            //foreach (var recipe in RecipeProtos.Values)
            //{
            //    recipe.Preload(index++);
            //    LDBTool.PreAddProto(recipe);
            //}

            //LDB.items.OnAfterDeserialize();
            //LDB.recipes.OnAfterDeserialize();

            //techs.dataArray = new List<TechProto>(TechProtos.Values).ToArray();
            //items.dataArray = new List<ItemProto>(ItemProtos.Values).ToArray();
            //recipes.dataArray = new List<RecipeProto>(RecipeProtos.Values).ToArray();


            PostFix(ref ItemProtos);

            //techs.Init(0);
            //items.Init(0);
            //recipes.Init(0);
        }

        public void PreFix()
        {
            LDB.strings.Select(2314).name = "剧毒液体海洋";
            LDB.strings.Select(2314).Name = "剧毒液体海洋";

            LDB.veins.Select(14).Name = "钨矿";
            LDB.veins.Select(14).name = "钨矿";

            LDB.milestones.Select(9).name = "钨";
            LDB.milestones.Select(9).Name = "钨";
            LDB.milestones.Select(9).defaultDesc = "你采集了钨矿,宇宙珍奇之一.它是一种用途广泛的新材料.";
            LDB.milestones.Select(9).DefaultDesc = "你采集了钨矿,宇宙珍奇之一.它是一种用途广泛的新材料.";

            LDB.items.Select(物品.单极磁石).ID = 6980;

            LDB.items.Select(物品.硫酸).ID = 6998;
            LDB.items.OnAfterDeserialize();
        }

        public void PostFix(ref Dictionary<int, ItemProto> itemProtos)
        {
            itemProtos[物品.一级制造台].prefabDesc.assemblerSpeed = 20000;
            itemProtos[物品.二级制造台].prefabDesc.assemblerRecipeType = (global::ERecipeType)ERecipeType.电路蚀刻;
            itemProtos[物品.二级制造台].prefabDesc.assemblerSpeed = 20000;
            itemProtos[物品.三级制造台].prefabDesc.assemblerRecipeType = (global::ERecipeType)ERecipeType.高精度加工;
            itemProtos[物品.三级制造台].prefabDesc.assemblerSpeed = 20000;
            itemProtos[物品.火力发电机].prefabDesc.useFuelPerTick = 200000;
            itemProtos[物品.采矿机].prefabDesc.minerPeriod = 600000;
            itemProtos[物品.大型采矿机].prefabDesc.minerPeriod = 300000;
            itemProtos[物品.位面熔炉].prefabDesc.assemblerSpeed = 40000;
            itemProtos[物品.化工厂].prefabDesc.assemblerSpeed = 20000;
            itemProtos[物品.原油采集站].prefabDesc.minerPeriod = 300000;
            itemProtos[物品.原油精炼厂].prefabDesc.assemblerSpeed = 20000;
            itemProtos[物品.研究站].prefabDesc.labResearchSpeed = 2;
            itemProtos[物品.研究站].prefabDesc.labAssembleSpeed = 2;
            itemProtos[物品.电磁轨道弹射器].prefabDesc.ejectorChargeFrame = 20;
            itemProtos[物品.电磁轨道弹射器].prefabDesc.ejectorColdFrame = 10;
            itemProtos[物品.垂直发射井].prefabDesc.siloChargeFrame = 24;
            itemProtos[物品.垂直发射井].prefabDesc.siloColdFrame = 6;
            itemProtos[物品.粒子对撞机].prefabDesc.assemblerSpeed = 40000;
            itemProtos[物品.卫星配电站].prefabDesc.powerCoverRadius = 2600.5f;
            itemProtos[物品.卫星配电站].prefabDesc.powerConnectDistance = 5300.5f;
            itemProtos[物品.电力感应塔].prefabDesc.powerConnectDistance = 44.5f;
            itemProtos[物品.电力感应塔].prefabDesc.powerCoverRadius = 20.5f;
            itemProtos[物品.火力发电机].prefabDesc.genEnergyPerTick = 200000;
            itemProtos[物品.太阳能板].prefabDesc.genEnergyPerTick = 40000;
            itemProtos[物品.地热发电机].prefabDesc.genEnergyPerTick = 400000;
            itemProtos[物品.聚变发电机].prefabDesc.genEnergyPerTick = 2500000;

            itemProtos[物品.风力涡轮机].prefabDesc.powerConnectDistance = 32.5f;
            itemProtos[物品.风力涡轮机].prefabDesc.powerCoverRadius = 14.9f;
            itemProtos[物品.风力涡轮机].prefabDesc.genEnergyPerTick = 50000;
            itemProtos[物品.人造恒星].prefabDesc.genEnergyPerTick = 120000000;

            itemProtos[物品.低速传送带].prefabDesc.beltSpeed = 3;
            itemProtos[物品.高速传送带].prefabDesc.beltSpeed = 5;
            itemProtos[物品.极速传送带].prefabDesc.beltSpeed = 10;
            itemProtos[物品.低速分拣器].prefabDesc.inserterSTT = 100000;
            itemProtos[物品.高速分拣器].prefabDesc.inserterSTT = 50000;
            itemProtos[物品.极速分拣器].prefabDesc.inserterSTT = 25000;
            itemProtos[物品.卫星配电站].prefabDesc.idleEnergyPerTick = 1200000;
            itemProtos[物品.卫星配电站].prefabDesc.workEnergyPerTick = 40000000;

            itemProtos[物品.电弧熔炉].prefabDesc.assemblerSpeed = 20000;
            itemProtos[物品.水泵].prefabDesc.minerPeriod = 360000;
            itemProtos[物品.无线输电塔].prefabDesc.powerConnectDistance = 90.5f;
        }

        private void PostAddDataAction()
        {

            foreach (var proto in LDB.techs.dataArray)
            {
                proto.Preload();
            }
            for (var i = 0; i < LDB.items.dataArray.Length; ++i)
            {
                LDB.items.dataArray[i].Preload(i);
            }
            for (var i = 0; i < LDB.recipes.dataArray.Length; ++i)
            {
                LDB.recipes.dataArray[i].Preload(i);
            }
            foreach (var proto in LDB.techs.dataArray)
            {
                proto.Preload2();
            }

            ItemProto.InitFluids();
            ItemProto.InitItemIds();
            ItemProto.InitFuelNeeds();
            ItemProto.InitItemIndices();
            ItemProto.InitMechaMaterials();

            var dataArray = LDB.items.dataArray;

            foreach (var proto in dataArray)
            {
                StorageComponent.itemIsFuel[proto.ID] = proto.HeatValue > 0L;
                StorageComponent.itemStackCount[proto.ID] = proto.StackSize;
            }

            var logo = GameObject.Find("UI Root/Overlay Canvas/Main Menu/dsp-logo");
            var image = logo.GetComponent<RawImage>();
            var rectTransform = logo.GetComponent<RectTransform>();

            var iconstr = Localization.language == Language.zhCN
                ? "Assets/texpack/中文图标"
                : "Assets/texpack/英文图标";

            image.texture = Resources.Load<Sprite>(iconstr).texture;


            rectTransform.sizeDelta = new Vector2(800f, 500f);
            rectTransform.anchoredPosition
                = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + 50f);
        }
    }
}
