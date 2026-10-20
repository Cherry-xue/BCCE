using BCCE.BCCECode.Characters.Ironclad.Powers;
using BCCE.BCCECode.Characters.Necrobinder.Powers;
using BCCE.BCCECode.Characters.Regent.Powers;
using BCCE.BCCECode.Characters.Silent.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BCCE.BCCECode;

// 随机增益池-按角色、通用、怪物、BCCE 分组存放，可单独取用，也可以取全部列表的合集
// 每条注释格式为「能力名-效果（层数影响）[持续时长]【怪物能力专有：玩家是否可用】」，均依据能力源码标注
public static class RandomPowerPool
{
    //定义一个委托：向指定生物施加一个增益
    public delegate Task Applier(PlayerChoiceContext choiceContext, Creature owner, Creature applier, CardModel cardSource);

    //定义铁甲战士的增益列表
    public static readonly IReadOnlyList<Applier> Ironclad =
    [
        // 巨像-本回合，你从易伤敌人处受到的伤害减少50%。（层数影响：层数不影响效果）[每回合结束减1层，持续层数个回合]
        (ctx, owner, applier, card) => PowerCmd.Apply<ColossusPower>(ctx, owner, 1, applier, card),

        // 腐化-所有技能牌耗能变为0。 所有技能牌在打出时被消耗。（层数影响：层数恒为1，不影响效果）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<CorruptionPower>(ctx, owner, 1, applier, card),

        // 绯红披风-在你的回合开始时，失去生命并获得格挡。（层数影响：回合开始时获得的格挡量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<CrimsonMantlePower>(ctx, owner, 1, applier, card),

        // 残酷-以上敌人受到额外伤害。（层数影响：易伤带来的额外伤害倍率，每1层提升1%）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<CrueltyPower>(ctx, owner, 1, applier, card),

        // 黑暗之拥-每当有一张牌被消耗时，抽1张牌。（层数影响：每消耗一张牌时的抽牌数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<DarkEmbracePower>(ctx, owner, 1, applier, card),

        // 恶魔形态-在你的回合开始时，获得3点力量。（层数影响：每回合开始时获得的力量层数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<DemonFormPower>(ctx, owner, 1, applier, card),

        // 火焰屏障-本回合你每次受到攻击时，反击造成4点伤害。（层数影响：受到攻击时反击的伤害）[本回合结束移除]
        (ctx, owner, applier, card) => PowerCmd.Apply<FlameBarrierPower>(ctx, owner, 1, applier, card),

        // 地狱狂徒-每当你抽到名字中有“打击”的牌时，对一名随机敌人打出这张牌。（层数影响：层数恒为1，不影响效果）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<HellraiserPower>(ctx, owner, 1, applier, card),

        // 狱火-在你的回合开始时，失去1点生命。每当你在自己的回合中失去生命时，对所有敌人造成4点伤害。（层数影响：失去生命时对所有敌人造成的伤害）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<InfernoPower>(ctx, owner, 1, applier, card),

        // 势不可当-每当你获得格挡时，对随机敌人造成6点伤害。（层数影响：获得格挡时对随机敌人造成的伤害）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<JuggernautPower>(ctx, owner, 1, applier, card),

        // 杂耍-将你在每回合打出的第三张攻击牌的复制品加入你的手牌。（层数影响：每回合第三张攻击牌的复制数量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<JugglingPower>(ctx, owner, 1, applier, card),

        // 连环拳-本回合，你的下一张攻击牌会多打出一次。（层数影响：层数不影响效果）[本回合结束移除]
        (ctx, owner, applier, card) => PowerCmd.Apply<OneTwoPunchPower>(ctx, owner, 1, applier, card),

        // 薪火之源-在每回合开始时获得1点能量。（层数影响：每回合额外获得的能量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<PyrePower>(ctx, owner, 1, applier, card),

        // 狂怒-本回合，你每打出一张攻击牌就获得3点格挡。（层数影响：每打出一张攻击牌获得的格挡量）[本回合结束移除]
        (ctx, owner, applier, card) => PowerCmd.Apply<RagePower>(ctx, owner, 1, applier, card),

        // 撕裂-每当你在自身回合失去生命时，获得1点力量。（层数影响：失去生命时获得的力量层数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<RupturePower>(ctx, owner, 1, applier, card),

        // 惊逃-在你的回合结束时，随机打出你手牌中的1张攻击牌攻击随机敌人。（层数影响：回合结束时随机打出的攻击牌数量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<StampedePower>(ctx, owner, 1, applier, card),

        // 肉盾-自身受到敌人伤害增加50%。盟友受到敌人伤害减少50%。（层数影响：给予盟友的被掩护层数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<TankPower>(ctx, owner, 1, applier, card),

        // 不动-每个回合你第一次从卡牌中获得格挡时，将数值翻倍。（层数影响：触发格挡翻倍所需的格挡阈值）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<UnmovablePower>(ctx, owner, 1, applier, card),

        // 凶恶-每当你给予易伤, 抽1张牌。（层数影响：给予易伤时的抽牌数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<ViciousPower>(ctx, owner, 1, applier, card),

        // 好勇斗狠-在你的回合开始时，将你弃牌堆的一张随机攻击牌放入你的手牌并将其在本场战斗中升级。（层数影响：回合开始时取回并升级的攻击牌数量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<AggressionPower>(ctx, owner, 1, applier, card),
    ];

    //定义静默猎手的增益列表
    public static readonly IReadOnlyList<Applier> Silent =
    [
        // 精准-小刀造成额外伤害。（层数影响：小刀造成的额外伤害）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<AccuracyPower>(ctx, owner, 1, applier, card),

        // 调制-每当你在本回合造成未被格挡的攻击伤害时，给予3层中毒。（层数影响：造成未格挡攻击伤害时给予的中毒层数）[本回合结束移除]
        (ctx, owner, applier, card) => PowerCmd.Apply<ConcoctPower>(ctx, owner, 1, applier, card),

        // 腐蚀波-你在本回合每抽到一张牌，就给予所有敌人2点中毒。（层数影响：每抽一张牌时给予所有敌人的中毒层数）[本回合结束移除]
        (ctx, owner, applier, card) => PowerCmd.Apply<CorrosiveWavePower>(ctx, owner, 1, applier, card),

        // 涂毒-每当你造成未被格挡的伤害时，给予1层中毒。（层数影响：造成未格挡伤害时给予的中毒层数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<EnvenomPower>(ctx, owner, 1, applier, card),

        // 刀扇-小刀会命中所有敌人。（层数影响：层数恒为1，不影响效果）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<FanOfKnivesPower>(ctx, owner, 1, applier, card),

        // 无尽刀刃-在你的回合开始时，在你的手牌中加入一张小刀。（层数影响：回合开始时加入手牌的小刀数量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<InfiniteBladesPower>(ctx, owner, 1, applier, card),

        // 谋划专家-当你打出技能牌时，该牌获得奇巧。（层数影响：层数恒为1，不影响效果）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<MasterPlannerPower>(ctx, owner, 1, applier, card),

        // 夜魇-在下个回合，将所选牌的3张复制品加入你的手牌。（层数影响：下回合加入手牌的复制品数量）[下回合抽牌时触发后移除]
        (ctx, owner, applier, card) => PowerCmd.Apply<NightmarePower>(ctx, owner, 1, applier, card),

        // 毒雾-在你的回合开始时，给予所有敌人2层中毒。（层数影响：回合开始时给予所有敌人的中毒层数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<NoxiousFumesPower>(ctx, owner, 1, applier, card),

        // 幻影之刃-小刀获得保留。 你每回合打出的第一张小刀伤害增加9。（层数影响：每回合第一张小刀的伤害增加量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<PhantomBladesPower>(ctx, owner, 1, applier, card),

        // 群蛇形态-你每打出一张牌，就对随机一名敌人造成4点伤害。（层数影响：每打出一张牌对随机敌人造成的伤害）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<SerpentFormPower>(ctx, owner, 1, applier, card),

        // 暗影步-在下个回合，攻击造成双倍伤害。（层数影响：下回合获得的双倍伤害层数）[下回合开始时移除]
        (ctx, owner, applier, card) => PowerCmd.Apply<ShadowStepPower>(ctx, owner, 1, applier, card),

        // 融入暗影-本回合你获得的格挡值翻倍。（层数影响：本回合格挡倍率，为2的层数次方）[本回合结束移除]
        (ctx, owner, applier, card) => PowerCmd.Apply<ShadowmeldPower>(ctx, owner, 1, applier, card),

        // 鬼祟-每当另一名玩家攻击敌人时，获得格挡。（层数影响：其他玩家攻击敌人时获得的格挡量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<SneakyPower>(ctx, owner, 1, applier, card),

        // 速行者-每当你在回合中抽到一张牌时，对所有敌人造成1点伤害。（层数影响：抽牌时对所有敌人造成的伤害）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<SpeedsterPower>(ctx, owner, 1, applier, card),

        // 狩猎-在战斗结束时额外获得1份卡牌奖励。（层数影响：层数不影响效果）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<TheHuntPower>(ctx, owner, 1, applier, card),

        // 必备工具-在你的回合开始时，抽1张牌并丢弃1张牌。（层数影响：每回合额外抽牌数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<ToolsOfTheTradePower>(ctx, owner, 1, applier, card),

        // 跟踪-虚弱的敌人从攻击牌中额外受到50%伤害。（层数影响：对虚弱敌人的额外伤害倍率，每1层提升1%）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<TrackingPower>(ctx, owner, 1, applier, card),

        // 计划妥当-在你的回合结束时，你不再丢弃你的手牌。（层数影响：层数恒为1，不影响效果）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<WellLaidPlansPower>(ctx, owner, 1, applier, card),

        // 触媒-中毒会多触发一次。（层数影响：层数不影响效果）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<AccelerantPower>(ctx, owner, 1, applier, card),
    ];

    //定义亡灵契约师的增益列表
    public static readonly IReadOnlyList<Applier> Necrobinder =
    [
        // 不谐合曲-所有玩家每抽33张牌，就会对随机一名敌人造成伤害。（层数影响：抽到牌时对敌人造成的伤害）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<CacophonyPower>(ctx, owner, 1, applier, card),

        // 钙化-奥斯提的攻击造成额外伤害。（层数影响：奥斯提造成的伤害增加量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<CalcifyPower>(ctx, owner, 1, applier, card),

        // 虚空之唤-在你的回合开始时，将1张随机牌添加到你的手牌并给予其虚无。（层数影响：抽牌时放入手牌的牌数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<CallOfTheVoidPower>(ctx, owner, 1, applier, card),

        // 倒数计时-在你的回合开始时，给予随机敌人6层灾厄。（层数影响：回合开始时给予随机敌人的灾厄层数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<CountdownPower>(ctx, owner, 1, applier, card),

        // 死亡之舞-每当你打出一张耗能为2或更高的牌时，获得7点格挡。（层数影响：打出高费牌时获得的格挡量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<DanseMacabrePower>(ctx, owner, 1, applier, card),

        // 领域-在你的回合开始时，获得1点能量并额外抽1张牌。（层数影响：回合开始时的额外抽牌数与额外能量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<DemesnePower>(ctx, owner, 1, applier, card),

        // 吞噬生命-每当你打出一张灵魂时，召唤1。（层数影响：打出灵魂时奥斯提获得的召唤数值）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<DevourLifePower>(ctx, owner, 1, applier, card),

        // 禁忌魔典-在战斗结束时，从你的牌组中移除一张牌。（层数影响：战斗结束时升级的牌数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<ForbiddenGrimoirePower>(ctx, owner, 1, applier, card),

        // 友谊-每回合开始时获得1点能量。（层数影响：每回合开始时额外获得的能量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<FriendshipPower>(ctx, owner, 1, applier, card),

        // 纠缠-每当你打出一张灵魂时，一个随机敌人失去3点生命。（层数影响：打出灵魂时对敌人造成的伤害）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<HauntPower>(ctx, owner, 1, applier, card),

        // 致死性-每回合的第一张攻击牌会造成50%额外伤害。（层数影响：每回合第一张攻击牌的额外伤害倍率，每1层提升1%）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<LethalityPower>(ctx, owner, 1, applier, card),

        // 亡灵精通-每当奥斯提失去生命值时，所有敌人失去等量生命值。（层数影响：自身生命变化量转化为对敌人伤害的倍率）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<NecroMasteryPower>(ctx, owner, 1, applier, card),

        // 书页风暴-每当你抽到一张虚无牌时，抽1张牌。（层数影响：抽到状态牌时额外抽牌数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<PagestormPower>(ctx, owner, 1, applier, card),

        // 死神形态-每当你的攻击造成伤害时，同时给予等量的灾厄。（层数影响：给予敌人的灾厄层数，为造成伤害乘以层数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<ReaperFormPower>(ctx, owner, 1, applier, card),

        // 哨卫模式-在你的回合开始时，将1张扫荡凝视加入你的手牌。（层数影响：抽牌时生成的牌数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<SentryModePower>(ctx, owner, 1, applier, card),

        // 厄运之衣-你每次给予灾厄时，获得3点格挡。（层数影响：给予敌人灾厄时获得的格挡量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<ShroudPower>(ctx, owner, 1, applier, card),

        // 血肉戏法-每当你给予一个敌人负面状态时，使其受到13点伤害。（层数影响：敌人获得减益时对其造成的伤害）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<SleightOfFleshPower>(ctx, owner, 1, applier, card),

        // 灵魂绑定-每当{ApplierName}生成一张灵魂时，将一张灵魂添加至你的抽牌堆。（层数影响：生成的灵魂数量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<SoulboundPower>(ctx, owner, 1, applier, card),

        // 灰烬之灵-每当你打出一张虚无牌时，获得4点格挡。（层数影响：打出虚无牌时获得的格挡量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<SpiritOfAshPower>(ctx, owner, 1, applier, card),

        // 下回合召唤-在你的下回合开始时，召唤2。（层数影响：下回合召唤奥斯提的数值）[下回合开始时触发后移除]
        (ctx, owner, applier, card) => PowerCmd.Apply<SummonNextTurnPower>(ctx, owner, 1, applier, card),

        // 幽冥之界-每当其他玩家在本回合造成攻击伤害时，给予等量的灾厄。（层数影响：给予敌人的灾厄层数，为造成伤害乘以层数）[本回合结束移除]
        (ctx, owner, applier, card) => PowerCmd.Apply<UnderworldPower>(ctx, owner, 1, applier, card),

        // 刺破帷幕-你打出的下一张虚无牌耗能为0。（层数影响：层数不影响效果）[每打出一张牌减1层，持续层数次]
        (ctx, owner, applier, card) => PowerCmd.Apply<VeilpiercerPower>(ctx, owner, 1, applier, card),
    ];

    //定义储君的增益列表
    public static readonly IReadOnlyList<Applier> Regent =
    [
        // 武器库-每当你生成一张牌，就获得1点力量。（层数影响：生成牌时获得的力量层数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<ArsenalPower>(ctx, owner, 1, applier, card),

        // 黑洞-每当你花费或获得{singleStarIcon}时，对所有敌人造成3点伤害。（层数影响：花费或获得辉星时对所有敌人造成的伤害）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<BlackHolePower>(ctx, owner, 1, applier, card),

        // 群星之子-每花费一点{singleStarIcon}，获得2点格挡。（层数影响：每花费1点辉星获得的格挡量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<ChildOfTheStarsPower>(ctx, owner, 1, applier, card),

        // 既定事项-下回合，将你抽牌堆中的3张牌放入你的手牌。（层数影响：下回合开始时放入手牌的牌数）[下回合抽牌时触发后移除]
        (ctx, owner, applier, card) => PowerCmd.Apply<ForegoneConclusionPower>(ctx, owner, 1, applier, card),

        // 熔炉-在你的回合开始时，铸造5。（层数影响：回合开始时铸造的数值）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<FurnacePower>(ctx, owner, 1, applier, card),

        // 创世纪-在你的回合开始时，获得{singleStarIcon}。（层数影响：回合开始时获得的辉星数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<GenesisPower>(ctx, owner, 1, applier, card),

        // 锤子时间-每当你铸造时，所有盟友也铸造相同数值。（层数影响：层数恒为1，不影响效果）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<HammerTimePower>(ctx, owner, 1, applier, card),

        // 王之凝视-每当你攻击敌人时，这名敌人在本回合失去1点力量。（层数影响：对敌人施加的减力量层数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<MonarchsGazePower>(ctx, owner, 1, applier, card),

        // 独白-每当你在本回合打出卡牌时，在本回合获得1点力量。（层数影响：层数不影响效果）[本回合结束移除]
        (ctx, owner, applier, card) => PowerCmd.Apply<MonologuePower>(ctx, owner, 1, applier, card),

        // 环绕轨道-你每花费4点能量，就获得1点能量。（层数影响：每触发一次返还的能量量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<OrbitPower>(ctx, owner, 1, applier, card),

        // 暗淡蓝点-当你在一回合内打出足够数量的牌时，就在下一回合开始时额外抽牌。（层数影响：打满5张牌后给予的下回合抽牌层数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<PaleBlueDotPower>(ctx, owner, 1, applier, card),

        // 招架-君王之剑现在能让你获得格挡。（层数影响：层数不影响效果）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<ParryPower>(ctx, owner, 1, applier, card),

        // 创世之柱-每当你生成卡牌时，获得{Amount}点格挡。（层数影响：生成牌时获得的格挡量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<PillarOfCreationPower>(ctx, owner, 1, applier, card),

        // 倒映-被格挡的伤害会反弹到攻击者身上。（层数影响：层数不影响效果）[每回合开始减1层，持续层数个回合]
        (ctx, owner, applier, card) => PowerCmd.Apply<ReflectPower>(ctx, owner, 1, applier, card),

        // 保留手牌-在2回合内保留你的手牌。（层数影响：层数不影响效果）[每回合结束减1层，持续层数个回合]
        (ctx, owner, applier, card) => PowerCmd.Apply<RetainHandPower>(ctx, owner, 1, applier, card),

        // 王国资产-在战斗结束时，获得25金币。（层数影响：战斗结束时获得的金币数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<RoyaltiesPower>(ctx, owner, 1, applier, card),

        // 追踪之刃-君王之剑现在会对所有敌人造成伤害。（层数影响：层数恒为1，不影响效果）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<SeekingEdgePower>(ctx, owner, 1, applier, card),

        // 光谱偏移-在你的回合开始时，将1张随机无色牌添加到你的手牌中。（层数影响：回合开始时生成的无色牌数量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<SpectrumShiftPower>(ctx, owner, 1, applier, card),

        // 下回合辉星-在下回合获得1{singleStarIcon}。（层数影响：下回合开始时获得的辉星数）[下回合开始时触发后移除]
        (ctx, owner, applier, card) => PowerCmd.Apply<StarNextTurnPower>(ctx, owner, 1, applier, card),

        // 剑圣-君王之剑获得重放1。（层数影响：君王之剑的额外重播次数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<SwordSagePower>(ctx, owner, 1, applier, card),

        // 封印王座-你每打出一张牌，获得{singleStarIcon}。（层数影响：每打出一张牌获得的辉星数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<TheSealedThronePower>(ctx, owner, 1, applier, card),

        // 暴政-在你的回合开始时，抽一张牌然后消耗你的一张手牌。（层数影响：回合开始时额外抽牌数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<TyrannyPower>(ctx, owner, 1, applier, card),

        // 虚空形态-每回合你打出的前2张牌会免费打出。（层数影响：每回合可免费打出的牌数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<VoidFormPower>(ctx, owner, 1, applier, card),
    ];

    //定义故障机器人的增益列表
    public static readonly IReadOnlyList<Applier> Defect =
    [
        // 吞噬暗影-在你的回合结束时，激发你最左侧的充能球。（层数影响：回合结束时触发的次数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<ConsumingShadowPower>(ctx, owner, 1, applier, card),

        // 冷却剂-在你的回合开始时，每有一种不同的充能球获得3点格挡。（层数影响：回合开始时获得的格挡量，为充能球数量乘以层数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<CoolantPower>(ctx, owner, 1, applier, card),

        // 创造性AI-在你的回合开始时，将一张随机能力牌添加到你的手牌。（层数影响：抽牌时生成的牌数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<CreativeAiPower>(ctx, owner, 1, applier, card),

        // 回响形态-你每回合打出的第一张牌会多打出一次。（层数影响：每回合可重复打出的牌数阈值）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<EchoFormPower>(ctx, owner, 1, applier, card),

        // 野性-每回合你第一次打出0{energyPrefix}攻击牌时，将其放回你的手牌中。（层数影响：层数不影响效果）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<FeralPower>(ctx, owner, 1, applier, card),

        // 集中-提升充能球的效力。（层数影响：充能球数值的增加量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<FocusPower>(ctx, owner, 1, applier, card),

        // 冰雹风暴-在你的回合结束时，如果你有冰霜充能球，则对所有敌人造成6点伤害。（层数影响：回合结束时对所有敌人造成的伤害）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<HailstormPower>(ctx, owner, 1, applier, card),

        // 休眠-本回合，你的冰霜充能球将会为所有盟友施加格挡。（层数影响：层数不影响效果）[每回合开始减1层，持续层数个回合]
        (ctx, owner, applier, card) => PowerCmd.Apply<HibernatePower>(ctx, owner, 1, applier, card),

        // 模仿学习-当另一名玩家打出能力牌时，你打出一张该牌的复制品。（层数影响：层数不影响效果）[每打出一张牌减1层，持续层数次]
        (ctx, owner, applier, card) => PowerCmd.Apply<ImitationLearningPower>(ctx, owner, 1, applier, card),

        // 迭代-每回合你第一次抽到状态牌时，抽更多牌。（层数影响：抽到状态牌时额外抽牌数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<IterationPower>(ctx, owner, 1, applier, card),

        // 引雷针-在下2个回合开始时，生成1个闪电充能球。（层数影响：层数不影响效果）[每回合开始减1层，持续层数个回合]
        (ctx, owner, applier, card) => PowerCmd.Apply<LightningRodPower>(ctx, owner, 1, applier, card),

        // 循环-在你的回合开始时，触发你最右侧充能球的被动能力。（层数影响：回合开始时激发充能球的次数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<LoopPower>(ctx, owner, 1, applier, card),

        // 机器学习-在你的回合开始时，额外抽1张牌。（层数影响：每回合额外抽牌数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<MachineLearningPower>(ctx, owner, 1, applier, card),

        // 一心化万-你的0{energyPrefix}攻击牌造成更多伤害。（层数影响：攻击造成的伤害增加量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<OneForAllPower>(ctx, owner, 1, applier, card),

        // 信号增强-你的下一张能力牌会额外打出一次。（层数影响：层数不影响效果）[触发一次后减1层，持续层数次]
        (ctx, owner, applier, card) => PowerCmd.Apply<SignalBoostPower>(ctx, owner, 1, applier, card),

        // 烟囱-每当你生成一张状态牌时，对所有敌人造成伤害。（层数影响：生成牌时对所有敌人造成的伤害）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<SmokestackPower>(ctx, owner, 1, applier, card),

        // 旋转工艺-在你的回合开始时，生成1个玻璃充能球。（层数影响：回合开始时生成的充能球数量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<SpinnerPower>(ctx, owner, 1, applier, card),

        // 雷暴-你每打出一张能力牌，就生成1个闪电充能球。（层数影响：每打出一张牌激发充能球的次数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<StormPower>(ctx, owner, 1, applier, card),

        // 子程序-你每次打出能力牌时，都获得{energyPrefix}。（层数影响：打出能力牌时获得的能量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<SubroutinePower>(ctx, owner, 1, applier, card),

        // 雷霆-每当你激发闪电充能球时，对命中的敌人造成8点伤害。（层数影响：激发闪电充能球时造成的伤害）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<ThunderPower>(ctx, owner, 1, applier, card),

        // 化废为宝-每当你生成状态牌的时候，随机生成1个充能球。（层数影响：生成状态牌时获得的充能球数量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<TrashToTreasurePower>(ctx, owner, 1, applier, card),
    ];

    //定义通用：任何角色都能用得上
    public static readonly IReadOnlyList<Applier> Common =
    [
        // 力量-力量会增加攻击牌造成的伤害。（层数影响：攻击牌造成的伤害增加量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<StrengthPower>(ctx, owner, 1, applier, card),

        // 敏捷-敏捷会增加从卡牌中获得的格挡。（层数影响：从卡牌获得的格挡增加量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<DexterityPower>(ctx, owner, 1, applier, card),

        // 活力-你的下一张攻击牌伤害增加。（层数影响：下一张攻击牌的伤害增加量）[下次攻击后清空]
        (ctx, owner, applier, card) => PowerCmd.Apply<VigorPower>(ctx, owner, 1, applier, card),

        // 荆棘-当被攻击命中时，反击造成伤害。（层数影响：被攻击命中时反击的伤害）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<ThornsPower>(ctx, owner, 1, applier, card),

        // 人工制品-免疫负面效果。（层数影响：可免疫负面效果的次数）[每抵挡一次负面效果减1层，持续层数次]
        (ctx, owner, applier, card) => PowerCmd.Apply<ArtifactPower>(ctx, owner, 1, applier, card),

        // 无实体-将本回合受到的所有伤害和生命减少效果降低为1。（层数影响：层数不影响效果）[每回合结束减1层，持续层数个回合]
        (ctx, owner, applier, card) => PowerCmd.Apply<IntangiblePower>(ctx, owner, 1, applier, card),

        // 覆甲-在你的回合结束时获得格挡。覆甲会在你的回合开始时减少1层。（层数影响：回合结束时获得的格挡量）[每回合开始减1层，持续层数个回合]
        (ctx, owner, applier, card) => PowerCmd.Apply<PlatingPower>(ctx, owner, 1, applier, card),

        // 再生-再生会在你的回合结束时回复相应生命。每回合再生的数值会减少1。（层数影响：回合结束时回复的生命值）[每回合结束减1层，持续层数个回合]
        (ctx, owner, applier, card) => PowerCmd.Apply<RegenPower>(ctx, owner, 1, applier, card),

        // 壁垒-格挡不会在你的回合开始时被移除。（层数影响：层数恒为1，不影响效果）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<BarricadePower>(ctx, owner, 1, applier, card),

        // 残影-你的下一回合开始时格挡不会消失。（层数影响：层数不影响效果）[每回合开始减1层，持续层数个回合]
        (ctx, owner, applier, card) => PowerCmd.Apply<BlurPower>(ctx, owner, 1, applier, card),

        // 余像-你每打出一张牌，都获得1点格挡。（层数影响：每打出一张牌获得的格挡量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<AfterimagePower>(ctx, owner, 1, applier, card),

        // 无惧疼痛-每当有一张牌被消耗时，获得3点格挡。（层数影响：每消耗一张牌时获得的格挡量）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<FeelNoPainPower>(ctx, owner, 1, applier, card),

        // 缓冲-阻止下一次你受到的生命值损伤。（层数影响：可免疫伤害的次数）[每抵挡一次伤害减1层，持续层数次]
        (ctx, owner, applier, card) => PowerCmd.Apply<BufferPower>(ctx, owner, 1, applier, card),

        // 双倍伤害-本回合，攻击造成双倍伤害。（层数影响：层数不影响效果）[每回合结束减1层，持续层数个回合]
        (ctx, owner, applier, card) => PowerCmd.Apply<DoubleDamagePower>(ctx, owner, 1, applier, card),

        // 爆发-本回合，你的下一张技能牌会多打出一次。（层数影响：层数不影响效果）[本回合结束移除]
        (ctx, owner, applier, card) => PowerCmd.Apply<BurstPower>(ctx, owner, 1, applier, card),

        // 免费攻击-你的下一张攻击牌耗能为0{energyPrefix}。（层数影响：层数不影响效果）[打出下一张攻击牌后减1层]
        (ctx, owner, applier, card) => PowerCmd.Apply<FreeAttackPower>(ctx, owner, 1, applier, card),

        // 免费技能-你的下一张技能牌耗能为0{energyPrefix}。（层数影响：层数不影响效果）[打出下一张技能牌后减1层]
        (ctx, owner, applier, card) => PowerCmd.Apply<FreeSkillPower>(ctx, owner, 1, applier, card),

        // 免费能力-你的下一张能力牌耗能为0 {energyPrefix}。（层数影响：层数不影响效果）[打出下一张能力牌后减1层]
        (ctx, owner, applier, card) => PowerCmd.Apply<FreePowerPower>(ctx, owner, 1, applier, card),

        // 下回合能量-在下回合额外获得能量。（层数影响：下回合额外获得的能量）[下回合开始时触发后移除]
        (ctx, owner, applier, card) => PowerCmd.Apply<EnergyNextTurnPower>(ctx, owner, 1, applier, card),

        // 下回合抽牌-在你的下回合开始时，额外抽1张牌。（层数影响：下回合开始时额外抽牌数）[下回合开始时触发后移除]
        (ctx, owner, applier, card) => PowerCmd.Apply<DrawCardsNextTurnPower>(ctx, owner, 1, applier, card),

        // 下回合格挡-在你的下个回合开始时，获得4点格挡。（层数影响：下回合开始时获得的格挡量）[下回合开始时触发后移除]
        (ctx, owner, applier, card) => PowerCmd.Apply<BlockNextTurnPower>(ctx, owner, 1, applier, card),
    ];

    //定义怪物：原版挂在怪物身上的正向能力，玩家也能吃下；每条末尾的【】标注了玩家拿到手是否真的有用
    public static readonly IReadOnlyList<Applier> Monster =
    [
        // 仪式-在你的回合结束时获得力量。（层数影响：回合结束时获得的力量层数）[永久]【玩家可用】
        (ctx, owner, applier, card) => PowerCmd.Apply<RitualPower>(ctx, owner, 1, applier, card),

        // 激怒-每当你打出一张技能牌时，获得2点力量。（层数影响：每打出一张技能牌获得的力量层数）[永久]【玩家可用】
        (ctx, owner, applier, card) => PowerCmd.Apply<EnragePower>(ctx, owner, 1, applier, card),

        // 高电压-这个生物的回合结束时，会获得2点力量。（层数影响：回合结束时获得的力量层数）[永久]【玩家可用】
        (ctx, owner, applier, card) => PowerCmd.Apply<HighVoltagePower>(ctx, owner, 1, applier, card),

        // 领地意识-这个生物在自身回合结束时，会获得1点力量。（层数影响：回合结束时获得的力量层数）[永久]【玩家可用】
        (ctx, owner, applier, card) => PowerCmd.Apply<TerritorialPower>(ctx, owner, 1, applier, card),

        // 胆小-这个生物每回合第一次被命中时，获得1点格挡。（层数影响：每回合首次被命中时获得的格挡量）[永久]【玩家可用】
        (ctx, owner, applier, card) => PowerCmd.Apply<SkittishPower>(ctx, owner, 1, applier, card),

        // 硬化外壳-这个生物每回合失去的生命值不会超过20点。（层数影响：每回合可承受的伤害上限）[永久]【玩家可用】
        (ctx, owner, applier, card) => PowerCmd.Apply<HardenedShellPower>(ctx, owner, 1, applier, card),

        // 难以杀灭-外骨骼虫受到的所有伤害和生命减少效果不会超过9点。（层数影响：单次受到的伤害上限）[永久]【玩家可用】
        (ctx, owner, applier, card) => PowerCmd.Apply<HardToKillPower>(ctx, owner, 1, applier, card),

        // 滑溜-这个生物下一次要失去生命值时，只会失去1点生命。（层数影响：层数不影响效果）[抵挡一次后减1层，持续层数次]【玩家可用】
        (ctx, owner, applier, card) => PowerCmd.Apply<SlipperyPower>(ctx, owner, 1, applier, card),

        // 天罚-每两个回合结束时，获得1层无实体。（层数影响：层数恒为1，不影响效果）[永久]【玩家可用】
        (ctx, owner, applier, card) => PowerCmd.Apply<NemesisPower>(ctx, owner, 1, applier, card),

        // 蟹之怒-当有盟友死亡时，这个生物获得5点力量和99点格挡。（层数影响：层数恒为1，不影响效果）[永久，持有者死亡时移除]【需同阵营其他单位死亡才触发，单人基本无效】
        (ctx, owner, applier, card) => PowerCmd.Apply<CrabRagePower>(ctx, owner, 1, applier, card),

        // 吮吸-这个生物每次造成未被格挡的伤害时，都会获得1点力量。（层数影响：每次造成未格挡伤害时获得的力量层数）[永久]【玩家可用】
        (ctx, owner, applier, card) => PowerCmd.Apply<SuckPower>(ctx, owner, 1, applier, card),
    ];

    //定义BCCE自有能力：我们mod自己的增益，层数与时长的依据同样来自各自源码
    public static readonly IReadOnlyList<Applier> BCCE =
    [
        // 聚力打击-在你的回合结束时，本回合每使用1点能量，获得力量，下个回合结束时，失去等量力量。（层数影响：本回合每使用1点能量给予的力量数值）[下个回合结束时失去力量并移除]
        (ctx, owner, applier, card) => PowerCmd.Apply<ChargedStrikePower>(ctx, owner, 1, applier, card),

        // 猩红渴求-每当有一名不是“爪牙”的敌人死亡，获得最大生命值。（层数影响：每有一名敌人死亡时获得的最大生命值）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<CrimsonThirstPower>(ctx, owner, 1, applier, card),

        // 回忆-在你的回合结束时，消耗所有状态牌，每消耗两张，获得1点力量。（层数影响：层数不影响效果）[本回合结束移除]
        (ctx, owner, applier, card) => PowerCmd.Apply<MemoryPower>(ctx, owner, 1, applier, card),

        // 弱肉强食-每当你获得力量时，使随机一名敌人失去等量力量。（层数影响：敌人失去力量效果的持续回合数）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<PredationPower>(ctx, owner, 1, applier, card),

        // 无声-在你的回合结束时，每有一张手牌，就获得敏捷，下个回合结束时，失去等量敏捷。（层数影响：每张手牌给予的敏捷数值）[永久]
        (ctx, owner, applier, card) => PowerCmd.Apply<AphoniaPower>(ctx, owner, 1, applier, card),

        // 奈何行-本回合内，你每打出3张灵魂，获得能量。（层数影响：每次触发获得的能量）[本回合结束移除]
        (ctx, owner, applier, card) => PowerCmd.Apply<HelplessWalkPower>(ctx, owner, 1, applier, card),
    ];
    private static IReadOnlyList<Applier> _all;

    //获取全部列表的合集：角色、通用、怪物一网打尽
    public static IReadOnlyList<Applier> All => _all ??= Ironclad
        .Concat(Silent)
        .Concat(Necrobinder)
        .Concat(Regent)
        .Concat(Defect)
        .Concat(Common)
        .Concat(Monster)
        .Concat(BCCE)
        .ToList();
}
