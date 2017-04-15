using anysdk;
using Assets.Scripts.Model;
using ExitGames.Client.Photon;
using GUIFramework;
using MobaHeros;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace Com.Game.Module
{
	public class LoginView_New : BaseView<LoginView_New>
	{
		private Transform BackButton;

		private Transform panel1;

		private Transform SNS;

		private Transform QQIcon;

		private Transform WeChat;

		private Transform XinlangIcon;

		public Transform SpeedRegister;

		private Transform RegisterAccount;

		private UIButton GetMessage;

		private UILabel GetMessageLabel;

		private UIButton GetMessage1;

		private UILabel GetMessageLabel1;

		private UIInput Message;

		private Transform Validate;

		private UILabel ValidateLabel;

		private Transform UseAccountLogin;

		public Transform ListLogin;

		private Transform panel2;

		private UIInput Account;

		private UIInput PIN;

		private Transform LoginButton;

		private UILabel lb_enterGame;

		private Transform ToRegistration;

		private Transform Forget;

		private Transform panel3;

		private UILabel YourUserNameLabel;

		private UIInput YourUserName;

		private UIInput RegistrationPIN;

		private UIInput PINAgain;

		private Transform Registration;

		private Transform ToLogin;

		private Transform Bg;

		private backButton record;

		private Transform mask;

		private UILabel versionLabel;

		private bool isPhoneValidateMode;

		private bool isCodeTimerRun;

		private bool isChangeCodeTimerRun;

		private int CodeTimerValue = 60;

		private int ChangeCodeTimerValue = 60;

		private UIToggle toggle;

		private CoroutineManager coroutineManager = new CoroutineManager();

		private bool isForgetMode;

		private Transform warn;

		private UILabel warnTitle;

		private UILabel warnLabel;

		private InitSDK hoolaiSDK;

		private Transform WarnText;

		private Transform ThirdParty;

		public bool isCancelAccount;

		public bool isFouceLoginByPhone;

		private List<string[]> userData;

		public LoginView_New()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Login/LoginView");
		}

		public override void Init()
		{
			base.Init();
			this.BackButton = this.transform.Find("BackButton");
			this.WarnText = this.transform.Find("WarnText");
			this.panel1 = this.transform.Find("panel1");
			this.SNS = this.transform.Find("SNS");
			this.ThirdParty = this.panel1.transform.Find("ThirdParty");
			this.QQIcon = this.panel1.transform.Find("ThirdParty/QQ");
			this.WeChat = this.panel1.transform.Find("ThirdParty/WeChat");
			this.XinlangIcon = this.SNS.Find("XinlangIcon");
			this.SpeedRegister = this.panel1.Find("SpeedRegister");
			this.RegisterAccount = this.panel1.Find("RegisterAccount");
			this.UseAccountLogin = this.panel1.Find("UseAccountLogin");
			this.ListLogin = this.panel1.Find("ListLogin");
			this.Bg = this.transform.Find("Bg");
			this.panel2 = this.transform.Find("panel2");
			this.Account = this.panel2.Find("Account").GetComponent<UIInput>();
			this.PIN = this.panel2.Find("PIN").GetComponent<UIInput>();
			this.LoginButton = this.panel2.Find("LoginButton");
			this.lb_enterGame = this.panel2.FindChild("LoginButton/Label").GetComponent<UILabel>();
			this.toggle = this.panel2.Find("Save").GetComponent<UIToggle>();
			this.ToRegistration = this.panel2.Find("ToRegistration");
			this.Forget = this.panel2.Find("Forget");
			this.panel3 = this.transform.Find("panel3");
			this.YourUserNameLabel = this.panel3.Find("YourUserName/Label").GetComponent<UILabel>();
			this.YourUserName = this.panel3.Find("YourUserName").GetComponent<UIInput>();
			this.RegistrationPIN = this.panel3.Find("RegistrationPIN").GetComponent<UIInput>();
			this.PINAgain = this.panel3.Find("PINAgain").GetComponent<UIInput>();
			this.Registration = this.panel3.Find("Registration");
			this.GetMessage = this.panel3.Find("GetMessage").GetComponent<UIButton>();
			this.GetMessageLabel = this.panel3.Find("GetMessage/Label").GetComponent<UILabel>();
			this.GetMessage1 = this.panel3.Find("GetMessage1").GetComponent<UIButton>();
			this.GetMessageLabel1 = this.panel3.Find("GetMessage1/Label").GetComponent<UILabel>();
			this.Message = this.panel3.Find("Message").GetComponent<UIInput>();
			this.Validate = this.panel3.Find("Validate");
			this.ValidateLabel = this.panel3.Find("Validate/Label").GetComponent<UILabel>();
			this.ToLogin = this.panel3.Find("ToLogin");
			this.mask = this.transform.Find("Mask");
			this.versionLabel = this.transform.Find("Version").GetComponent<UILabel>();
			this.warn = this.transform.Find("Warn");
			this.warnTitle = this.warn.Find("WarnTitile").GetComponent<UILabel>();
			this.warnLabel = this.warn.Find("WarnaLabel").GetComponent<UILabel>();
			if ((GlobalSettings.isLoginByHoolaiSDK || GlobalSettings.isLoginByAnySDK || GlobalSettings.isLoginByLDSDK) && !this.isFouceLoginByPhone)
			{
				this.SDKLoginFail();
			}
			else
			{
				this.mask.gameObject.SetActive(false);
				this.WarnText.gameObject.SetActive(false);
			}
			if (GlobalSettings.needThirdLogin || this.isFouceLoginByPhone)
			{
				this.ThirdParty.gameObject.SetActive(true);
				this.RegisterAccount.localPosition = new Vector3(10f, 42f, 0f);
				this.SpeedRegister.localPosition = new Vector3(10f, -103f, 0f);
			}
			else
			{
				this.ThirdParty.gameObject.SetActive(false);
				this.RegisterAccount.localPosition = new Vector3(10f, -50f, 0f);
				this.SpeedRegister.localPosition = new Vector3(10f, -195f, 0f);
			}
			UIEventListener.Get(this.LoginButton.gameObject).onClick = new UIEventListener.VoidDelegate(this.LoginGame);
			UIEventListener.Get(this.BackButton.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickBackButton);
			UIEventListener.Get(this.RegisterAccount.gameObject).onClick = new UIEventListener.VoidDelegate(this.clickRegisterAccount);
			UIEventListener.Get(this.UseAccountLogin.gameObject).onClick = new UIEventListener.VoidDelegate(this.clickUseAccountLogin);
			UIEventListener.Get(this.Registration.gameObject).onClick = new UIEventListener.VoidDelegate(this.clickRegistration);
			UIEventListener.Get(this.SpeedRegister.gameObject).onClick = new UIEventListener.VoidDelegate(this.clickSpeedRegister);
			EventDelegate.Add(this.YourUserName.onChange, new EventDelegate.Callback(this.CheckPhoneNumber));
			UIEventListener.Get(this.GetMessage.gameObject).onClick = new UIEventListener.VoidDelegate(this.TryGetMessage);
			UIEventListener.Get(this.GetMessage1.gameObject).onClick = new UIEventListener.VoidDelegate(this.TryGetMessage);
			UIEventListener.Get(this.Validate.gameObject).onClick = new UIEventListener.VoidDelegate(this.CheckPhoneAndMessage);
			UIEventListener.Get(this.ToLogin.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnToLogin);
			UIEventListener.Get(this.ToRegistration.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnToRegistration);
			UIEventListener.Get(this.Forget.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnForget);
			UIEventListener.Get(this.QQIcon.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnQQIcon);
			UIEventListener.Get(this.WeChat.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnWeChat);
			this.ShowVersionInfo();
		}

		public override void HandleAfterOpenView()
		{
			base.HandleAfterOpenView();
			this.RequestFontTexture();
			AutoTestController.InvokeTestLogic(AutoTestTag.Login, new Action(this.TestAutoLogin), 1f);
		}

		private void RequestFontTexture()
		{
			Font trueTypeFont = this.GetMessageLabel.trueTypeFont;
			Font trueTypeFont2 = this.ValidateLabel.GetComponent<UILabel>().trueTypeFont;
			string characters = "下次登录不需要再输入密码请输入预约的手机号输入验证码获得验证码欢迎您——小梦的梦想赞助商。小梦将竭尽全力为您带来最有格调的游戏体验。登陆成功注销尾号abcdefghijklmnopqrstuvwxyz0123456789QWERTYUIOPASDFGHJKLZXCVBNM/（）()小兵之刃速度之靴【加速】杀小兵、英雄、野怪，都可以获得金币在商店附近时，点击左下角打开商店快捷购买推荐装备，点击图标即可直接购买士兵旋风斩银爵电脑敌方防御塔已被摧毁我方派出超级兵《现代汉语常用字表》之一常用字(2500字)笔画顺序表一画一乙二画二十丁厂七卜人入八九几儿了力乃刀又三画三于干亏士工土才寸下大丈与万上小口巾山千乞川亿个勺久凡及夕丸么广亡门义之尸弓己已子卫也女飞刃习叉马乡四画丰王井开夫天无元专云扎艺木五支厅不太犬区历尤友匹车巨牙屯比互切瓦止少日中冈贝内水见午牛手毛气升长仁什片仆化仇币仍仅斤爪反介父从今凶分乏公仓月氏勿欠风丹匀乌凤勾文六方火为斗忆订计户认心尺引丑巴孔队办以允予劝双书幻五画玉刊示末未击打巧正扑扒功扔去甘世古节本术可丙左厉右石布龙平灭轧东卡北占业旧帅归且旦目叶甲申叮电号田由史只央兄叼叫另叨叹四生失禾丘付仗代仙们仪白仔他斥瓜乎丛令用甩印乐句匆册犯外处冬鸟务包饥主市立闪兰半汁汇头汉宁穴它讨写让礼训必议讯记永司尼民出辽奶奴加召皮边发孕圣对台矛纠母幼丝六画式刑动扛寺吉扣考托老执巩圾扩扫地扬场耳共芒亚芝朽朴机权过臣再协西压厌在有百存而页匠夸夺灰达列死成夹轨邪划迈毕至此贞师尘尖劣光当早吐吓虫曲团同吊吃因吸吗屿帆岁回岂刚则肉网年朱先丢舌竹迁乔伟传乒乓休伍伏优伐延件任伤价份华仰仿伙伪自血向似后行舟全会杀合兆企众爷伞创肌朵杂危旬旨负各名多争色壮冲冰庄庆亦刘齐交次衣产决充妄闭问闯羊并关米灯州汗污江池汤忙兴宇守宅字安讲军许论农讽设访寻那迅尽导异孙阵阳收阶阴防奸如妇好她妈戏羽观欢买红纤级约纪驰巡七画寿弄麦形进戒吞远违运扶抚坛技坏扰拒找批扯址走抄坝贡攻赤折抓扮抢孝均抛投坟抗坑坊抖护壳志扭块声把报却劫芽花芹芬苍芳严芦劳克苏杆杠杜材村杏极李杨求更束豆两丽医辰励否还歼来连步坚旱盯呈时吴助县里呆园旷围呀吨足邮男困吵串员听吩吹呜吧吼别岗帐财针钉告我乱利秃秀私每兵估体何但伸作伯伶佣低你住位伴身皂佛近彻役返余希坐谷妥含邻岔肝肚肠龟免狂犹角删条卵岛迎饭饮系言冻状亩况床库疗应冷这序辛弃冶忘闲间闷判灶灿弟汪沙汽沃泛沟没沈沉怀忧快完宋宏牢究穷灾良证启评补初社识诉诊词译君灵即层尿尾迟局改张忌际陆阿陈阻附妙妖妨努忍劲鸡驱纯纱纳纲驳纵纷纸纹纺驴纽八画奉玩环武青责现表规抹拢拔拣担坦押抽拐拖拍者顶拆拥抵拘势抱垃拉拦拌幸招坡披拨择抬其取苦若茂苹苗英范直茄茎茅林枝杯柜析板松枪构杰述枕丧或画卧事刺枣雨卖矿码厕奔奇奋态欧垄妻轰顷转斩轮软到非叔肯齿些虎虏肾贤尚旺具果味昆国昌畅明易昂典固忠咐呼鸣咏呢岸岩帖罗帜岭凯败贩购图钓制知垂牧物乖刮秆和季委佳侍供使例版侄侦侧凭侨佩货依的迫质欣征往爬彼径所舍金命斧爸采受乳贪念贫肤肺肢肿胀朋股肥服胁周昏鱼兔忽狗备饰饱饲变京享店夜庙府底剂郊废净盲放刻育闸闹郑券卷单炒炊炕炎炉沫浅法泄河沾泪油泊沿泡注泻泳泥沸波泼泽治怖性怕怜怪学宝宗定宜审宙官空帘实试郎诗肩房诚衬衫视话诞询该详建肃录隶居届刷屈弦承孟孤陕降限妹姑姐姓始驾参艰线练组细驶织终驻驼绍经贯九画奏春帮珍玻毒型挂封持项垮挎城挠政赴赵挡挺括拴拾挑指垫挣挤拼挖按挥挪某甚革荐巷带草茧茶荒茫荡荣故胡南药标枯柄栋相查柏柳柱柿栏树要咸歪研砖厘厚砌砍面耐耍牵残殃轻鸦皆背战点临览竖省削尝是盼眨哄显哑冒映星昨畏趴胃贵界虹虾蚁思蚂虽品咽骂哗咱响哈咬咳哪炭峡罚贱贴骨钞钟钢钥钩卸缸拜看矩怎牲选适秒香种秋科重复竿段便俩贷顺修保促侮俭俗俘信皇泉鬼侵追俊盾待律很须叙剑逃食盆胆胜胞胖脉勉狭狮独狡狱狠贸怨急饶蚀饺饼弯将奖哀亭亮度迹庭疮疯疫疤姿亲音帝施闻阀阁差养美姜叛送类迷前首逆总炼炸炮烂剃洁洪洒浇浊洞测洗活派洽染济洋洲浑浓津恒恢恰恼恨举觉宣室宫宪突穿窃客冠语扁袄祖神祝误诱说诵垦退既屋昼费陡眉孩除险院娃姥姨姻娇怒架贺盈勇怠柔垒绑绒结绕骄绘给络骆绝绞统十画耕耗艳泰珠班素蚕顽盏匪捞栽捕振载赶起盐捎捏埋捉捆捐损都哲逝捡换挽热恐壶挨耻耽恭莲莫荷获晋恶真框桂档桐株桥桃格校核样根索哥速逗栗配翅辱唇夏础破原套逐烈殊顾轿较顿毙致柴桌虑监紧党晒眠晓鸭晃晌晕蚊哨哭恩唤啊唉罢峰圆贼贿钱钳钻铁铃铅缺氧特牺造乘敌秤租积秧秩称秘透笔笑笋债借值倚倾倒倘俱倡候俯倍倦健臭射躬息徒徐舰舱般航途拿爹爱颂翁脆脂胸胳脏胶脑狸狼逢留皱饿恋桨浆衰高席准座脊症病疾疼疲效离唐资凉站剖竞部旁旅畜阅羞瓶拳粉料益兼烤烘烦烧烛烟递涛浙涝酒涉消浩海涂浴浮流润浪浸涨烫涌悟悄悔悦害宽家宵宴宾窄容宰案请朗诸读扇袜袖袍被祥课谁调冤谅谈谊剥恳展剧屑弱陵陶陷陪娱娘通能难预桑绢绣验继十一画球理捧堵描域掩捷排掉堆推掀授教掏掠培接控探据掘职基著勒黄萌萝菌菜萄菊萍菠营械梦梢梅检梳梯桶救副票戚爽聋袭盛雪辅辆虚雀堂常匙晨睁眯眼悬野啦晚啄距跃略蛇累唱患唯崖崭崇圈铜铲银甜梨犁移笨笼笛符第敏做袋悠偿偶偷您售停偏假得衔盘船斜盒鸽悉欲彩领脚脖脸脱象够猜猪猎猫猛馅馆凑减毫麻痒痕廊康庸鹿盗章竟商族旋望率着盖粘粗粒断剪兽清添淋淹渠渐混渔淘液淡深婆梁渗情惜惭悼惧惕惊惨惯寇寄宿窑密谋谎祸谜逮敢屠弹随蛋隆隐婚婶颈绩绪续骑绳维绵绸绿十二画琴斑替款堪搭塔越趁趋超提堤博揭喜插揪搜煮援裁搁搂搅握揉斯期欺联散惹葬葛董葡敬葱落朝辜葵棒棋植森椅椒棵棍棉棚棕惠惑逼厨厦硬确雁殖裂雄暂雅辈悲紫辉敞赏掌晴暑最量喷晶喇遇喊景践跌跑遗蛙蛛蜓喝喂喘喉幅帽赌赔黑铸铺链销锁锄锅锈锋锐短智毯鹅剩稍程稀税筐等筑策筛筒答筋筝傲傅牌堡集焦傍储奥街惩御循艇舒番释禽腊脾腔鲁猾猴然馋装蛮就痛童阔善羡普粪尊道曾焰港湖渣湿温渴滑湾渡游滋溉愤慌惰愧愉慨割寒富窜窝窗遍裕裤裙谢谣谦属屡强粥疏隔隙絮嫂登缎缓编骗缘三画瑞魂肆摄摸填搏塌鼓摆携搬摇搞塘摊蒜勤鹊蓝墓幕蓬蓄蒙蒸献禁楚想槐榆楼概赖酬感碍碑碎碰碗碌雷零雾雹输督龄鉴睛睡睬鄙愚暖盟歇暗照跨跳跪路跟遣蛾蜂嗓置罪罩错锡锣锤锦键锯矮辞稠愁筹签简毁舅鼠催傻像躲微愈遥腰腥腹腾腿触解酱痰廉新韵意粮数煎塑慈煤煌满漠源滤滥滔溪溜滚滨粱滩慎誉塞谨福群殿辟障嫌嫁叠缝缠十四画静碧璃墙撇嘉摧截誓境摘摔聚蔽慕暮蔑模榴榜榨歌遭酷酿酸磁愿需弊裳颗嗽蜻蜡蝇蜘赚锹锻舞稳算箩管僚鼻魄貌膜膊膀鲜疑馒裹敲豪膏遮腐瘦辣竭端旗精歉熄熔漆漂漫滴演漏慢寨赛察蜜谱嫩翠熊凳骡缩";
			string characters2 = "记住密码进入游戏没有账号?立即注册!忘记密码?已有账号?立即登录!流畅拥挤爆满维护选择服务器确定推荐服务器上次登录服务器服务器列表下载资源加载资源abcdefghijklmnopqrstuvwxyz0123456789QWERTYUIOPASDFGHJKLZXCVBNM()%.（“”）欢迎来到魔霸英雄泳装小妖将全程为您提供游戏引导服务我初次玩魔霸英雄完成引导即送玩过魔霸英雄你真的舍得拒绝小妖手把手的指导吗?加载过程不耗流量添加官方梦的点滴了解最新游戏资讯点击屏幕返回最佳游戏姿势左右手拇指分别覆盖左右屏幕开启战斗内商店如何获得金币推荐进攻法术防御辅助快捷购买推荐装备点击地面即可移动移动到我方防御塔前点击目标，即可选中技能加点与升级点击图标释放技能继续游戏《现代汉语常用字表》之一常用字(2500字)笔画顺序表一画一乙二画二十丁厂七卜人入八九几儿了力乃刀又三画三于干亏士工土才寸下大丈与万上小口巾山千乞川亿个勺久凡及夕丸么广亡门义之尸弓己已子卫也女飞刃习叉马乡四画丰王井开夫天无元专云扎艺木五支厅不太犬区历尤友匹车巨牙屯比互切瓦止少日中冈贝内水见午牛手毛气升长仁什片仆化仇币仍仅斤爪反介父从今凶分乏公仓月氏勿欠风丹匀乌凤勾文六方火为斗忆订计户认心尺引丑巴孔队办以允予劝双书幻五画玉刊示末未击打巧正扑扒功扔去甘世古节本术可丙左厉右石布龙平灭轧东卡北占业旧帅归且旦目叶甲申叮电号田由史只央兄叼叫另叨叹四生失禾丘付仗代仙们仪白仔他斥瓜乎丛令用甩印乐句匆册犯外处冬鸟务包饥主市立闪兰半汁汇头汉宁穴它讨写让礼训必议讯记永司尼民出辽奶奴加召皮边发孕圣对台矛纠母幼丝六画式刑动扛寺吉扣考托老执巩圾扩扫地扬场耳共芒亚芝朽朴机权过臣再协西压厌在有百存而页匠夸夺灰达列死成夹轨邪划迈毕至此贞师尘尖劣光当早吐吓虫曲团同吊吃因吸吗屿帆岁回岂刚则肉网年朱先丢舌竹迁乔伟传乒乓休伍伏优伐延件任伤价份华仰仿伙伪自血向似后行舟全会杀合兆企众爷伞创肌朵杂危旬旨负各名多争色壮冲冰庄庆亦刘齐交次衣产决充妄闭问闯羊并关米灯州汗污江池汤忙兴宇守宅字安讲军许论农讽设访寻那迅尽导异孙阵阳收阶阴防奸如妇好她妈戏羽观欢买红纤级约纪驰巡七画寿弄麦形进戒吞远违运扶抚坛技坏扰拒找批扯址走抄坝贡攻赤折抓扮抢孝均抛投坟抗坑坊抖护壳志扭块声把报却劫芽花芹芬苍芳严芦劳克苏杆杠杜材村杏极李杨求更束豆两丽医辰励否还歼来连步坚旱盯呈时吴助县里呆园旷围呀吨足邮男困吵串员听吩吹呜吧吼别岗帐财针钉告我乱利秃秀私每兵估体何但伸作伯伶佣低你住位伴身皂佛近彻役返余希坐谷妥含邻岔肝肚肠龟免狂犹角删条卵岛迎饭饮系言冻状亩况床库疗应冷这序辛弃冶忘闲间闷判灶灿弟汪沙汽沃泛沟没沈沉怀忧快完宋宏牢究穷灾良证启评补初社识诉诊词译君灵即层尿尾迟局改张忌际陆阿陈阻附妙妖妨努忍劲鸡驱纯纱纳纲驳纵纷纸纹纺驴纽八画奉玩环武青责现表规抹拢拔拣担坦押抽拐拖拍者顶拆拥抵拘势抱垃拉拦拌幸招坡披拨择抬其取苦若茂苹苗英范直茄茎茅林枝杯柜析板松枪构杰述枕丧或画卧事刺枣雨卖矿码厕奔奇奋态欧垄妻轰顷转斩轮软到非叔肯齿些虎虏肾贤尚旺具果味昆国昌畅明易昂典固忠咐呼鸣咏呢岸岩帖罗帜岭凯败贩购图钓制知垂牧物乖刮秆和季委佳侍供使例版侄侦侧凭侨佩货依的迫质欣征往爬彼径所舍金命斧爸采受乳贪念贫肤肺肢肿胀朋股肥服胁周昏鱼兔忽狗备饰饱饲变京享店夜庙府底剂郊废净盲放刻育闸闹郑券卷单炒炊炕炎炉沫浅法泄河沾泪油泊沿泡注泻泳泥沸波泼泽治怖性怕怜怪学宝宗定宜审宙官空帘实试郎诗肩房诚衬衫视话诞询该详建肃录隶居届刷屈弦承孟孤陕降限妹姑姐姓始驾参艰线练组细驶织终驻驼绍经贯九画奏春帮珍玻毒型挂封持项垮挎城挠政赴赵挡挺括拴拾挑指垫挣挤拼挖按挥挪某甚革荐巷带草茧茶荒茫荡荣故胡南药标枯柄栋相查柏柳柱柿栏树要咸歪研砖厘厚砌砍面耐耍牵残殃轻鸦皆背战点临览竖省削尝是盼眨哄显哑冒映星昨畏趴胃贵界虹虾蚁思蚂虽品咽骂哗咱响哈咬咳哪炭峡罚贱贴骨钞钟钢钥钩卸缸拜看矩怎牲选适秒香种秋科重复竿段便俩贷顺修保促侮俭俗俘信皇泉鬼侵追俊盾待律很须叙剑逃食盆胆胜胞胖脉勉狭狮独狡狱狠贸怨急饶蚀饺饼弯将奖哀亭亮度迹庭疮疯疫疤姿亲音帝施闻阀阁差养美姜叛送类迷前首逆总炼炸炮烂剃洁洪洒浇浊洞测洗活派洽染济洋洲浑浓津恒恢恰恼恨举觉宣室宫宪突穿窃客冠语扁袄祖神祝误诱说诵垦退既屋昼费陡眉孩除险院娃姥姨姻娇怒架贺盈勇怠柔垒绑绒结绕骄绘给络骆绝绞统十画耕耗艳泰珠班素蚕顽盏匪捞栽捕振载赶起盐捎捏埋捉捆捐损都哲逝捡换挽热恐壶挨耻耽恭莲莫荷获晋恶真框桂档桐株桥桃格校核样根索哥速逗栗配翅辱唇夏础破原套逐烈殊顾轿较顿毙致柴桌虑监紧党晒眠晓鸭晃晌晕蚊哨哭恩唤啊唉罢峰圆贼贿钱钳钻铁铃铅缺氧特牺造乘敌秤租积秧秩称秘透笔笑笋债借值倚倾倒倘俱倡候俯倍倦健臭射躬息徒徐舰舱般航途拿爹爱颂翁脆脂胸胳脏胶脑狸狼逢留皱饿恋桨浆衰高席准座脊症病疾疼疲效离唐资凉站剖竞部旁旅畜阅羞瓶拳粉料益兼烤烘烦烧烛烟递涛浙涝酒涉消浩海涂浴浮流润浪浸涨烫涌悟悄悔悦害宽家宵宴宾窄容宰案请朗诸读扇袜袖袍被祥课谁调冤谅谈谊剥恳展剧屑弱陵陶陷陪娱娘通能难预桑绢绣验继十一画球理捧堵描域掩捷排掉堆推掀授教掏掠培接控探据掘职基著勒黄萌萝菌菜萄菊萍菠营械梦梢梅检梳梯桶救副票戚爽聋袭盛雪辅辆虚雀堂常匙晨睁眯眼悬野啦晚啄距跃略蛇累唱患唯崖崭崇圈铜铲银甜梨犁移笨笼笛符第敏做袋悠偿偶偷您售停偏假得衔盘船斜盒鸽悉欲彩领脚脖脸脱象够猜猪猎猫猛馅馆凑减毫麻痒痕廊康庸鹿盗章竟商族旋望率着盖粘粗粒断剪兽清添淋淹渠渐混渔淘液淡深婆梁渗情惜惭悼惧惕惊惨惯寇寄宿窑密谋谎祸谜逮敢屠弹随蛋隆隐婚婶颈绩绪续骑绳维绵绸绿十二画琴斑替款堪搭塔越趁趋超提堤博揭喜插揪搜煮援裁搁搂搅握揉斯期欺联散惹葬葛董葡敬葱落朝辜葵棒棋植森椅椒棵棍棉棚棕惠惑逼厨厦硬确雁殖裂雄暂雅辈悲紫辉敞赏掌晴暑最量喷晶喇遇喊景践跌跑遗蛙蛛蜓喝喂喘喉幅帽赌赔黑铸铺链销锁锄锅锈锋锐短智毯鹅剩稍程稀税筐等筑策筛筒答筋筝傲傅牌堡集焦傍储奥街惩御循艇舒番释禽腊脾腔鲁猾猴然馋装蛮就痛童阔善羡普粪尊道曾焰港湖渣湿温渴滑湾渡游滋溉愤慌惰愧愉慨割寒富窜窝窗遍裕裤裙谢谣谦属屡强粥疏隔隙絮嫂登缎缓编骗缘三画瑞魂肆摄摸填搏塌鼓摆携搬摇搞塘摊蒜勤鹊蓝墓幕蓬蓄蒙蒸献禁楚想槐榆楼概赖酬感碍碑碎碰碗碌雷零雾雹输督龄鉴睛睡睬鄙愚暖盟歇暗照跨跳跪路跟遣蛾蜂嗓置罪罩错锡锣锤锦键锯矮辞稠愁筹签简毁舅鼠催傻像躲微愈遥腰腥腹腾腿触解酱痰廉新韵意粮数煎塑慈煤煌满漠源滤滥滔溪溜滚滨粱滩慎誉塞谨福群殿辟障嫌嫁叠缝缠十四画静碧璃墙撇嘉摧截誓境摘摔聚蔽慕暮蔑模榴榜榨歌遭酷酿酸磁愿需弊裳颗嗽蜻蜡蝇蜘赚锹锻舞稳算箩管僚鼻魄貌膜膊膀鲜疑馒裹敲豪膏遮腐瘦辣竭端旗精歉熄熔漆漂漫滴演漏慢寨赛察蜜谱嫩翠熊凳骡缩";
			trueTypeFont.RequestCharactersInTexture(characters);
			trueTypeFont2.RequestCharactersInTexture(characters2);
		}

		private void TestAutoLogin()
		{
			if (!string.IsNullOrEmpty(this.Account.value) && !string.IsNullOrEmpty(this.PIN.value))
			{
				this.LoginGame(null);
				return;
			}
			this.clickSpeedRegister(null);
		}

		public override void RegisterUpdateHandler()
		{
			MVC_MessageManager.AddListener_view(MobaMasterCode.GuestUpgrade, new MobaMessageFunc(this.OnGuestUpgrade));
			MVC_MessageManager.AddListener_view(MobaMasterCode.Register, new MobaMessageFunc(this.OnRegister));
			MVC_MessageManager.AddListener_view(MobaMasterCode.Login, new MobaMessageFunc(this.OnLogin));
			MVC_MessageManager.AddListener_view(MobaMasterCode.GetPhoneCode, new MobaMessageFunc(this.OnGetPhoneCode));
			MVC_MessageManager.AddListener_view(MobaMasterCode.CheckPhoneAndCode, new MobaMessageFunc(this.OnCheckPhoneAndCode));
			MVC_MessageManager.AddListener_view(MobaMasterCode.FindMyAccountPasswd, new MobaMessageFunc(this.OnFindMyAccountPasswd));
			MVC_MessageManager.AddListener_view(MobaMasterCode.ModifyAccountPasswd, new MobaMessageFunc(this.OnModifyAccountPasswd));
			MVC_MessageManager.AddListener_view(MobaMasterCode.LoginByPlatformUid, new MobaMessageFunc(this.OnLoginByPlatformUid));
			MVC_MessageManager.AddListener_view(MobaMasterCode.LoginByChannelId, new MobaMessageFunc(this.OnLoginByPlatformUid));
			GlobalObject.Instance.SetCanPause(true);
			if (Singleton<LoginView_New>.Instance.isFouceLoginByPhone)
			{
				this.RefreshUI();
			}
			else if (GlobalSettings.isLoginByHoolaiSDK)
			{
				this.UpdateLoginView(false);
				this.SDKLoginFail();
				InitSDK.instance.StartSDKLogin();
			}
			else if (GlobalSettings.isLoginByAnySDK)
			{
				this.UpdateLoginView(false);
				this.SDKLoginFail();
				InitSDK.instance.StartAnySDKLogin();
			}
			else
			{
				this.RefreshUI();
			}
		}

		public override void CancelUpdateHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaMasterCode.GuestUpgrade, new MobaMessageFunc(this.OnGuestUpgrade));
			MVC_MessageManager.RemoveListener_model(MobaMasterCode.Register, new MobaMessageFunc(this.OnRegister));
			MVC_MessageManager.RemoveListener_model(MobaMasterCode.Login, new MobaMessageFunc(this.OnLogin));
			MVC_MessageManager.RemoveListener_model(MobaMasterCode.GetPhoneCode, new MobaMessageFunc(this.OnGetPhoneCode));
			MVC_MessageManager.RemoveListener_model(MobaMasterCode.CheckPhoneAndCode, new MobaMessageFunc(this.OnCheckPhoneAndCode));
			MVC_MessageManager.RemoveListener_model(MobaMasterCode.FindMyAccountPasswd, new MobaMessageFunc(this.OnFindMyAccountPasswd));
			MVC_MessageManager.RemoveListener_model(MobaMasterCode.ModifyAccountPasswd, new MobaMessageFunc(this.OnModifyAccountPasswd));
			MVC_MessageManager.RemoveListener_model(MobaMasterCode.LoginByPlatformUid, new MobaMessageFunc(this.OnLoginByPlatformUid));
			MVC_MessageManager.RemoveListener_model(MobaMasterCode.LoginByChannelId, new MobaMessageFunc(this.OnLoginByPlatformUid));
		}

		public override void RefreshUI()
		{
			this.UpdateLoginView(true);
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void ShowVersionInfo()
		{
			string text = string.Empty;
			ClientData clientData = ModelManager.Instance.Get_ClientData_X();
			text = text + "v" + clientData.AppVersion;
			this.versionLabel.text = text;
		}

		private void UpdateLoginView(bool isShow = true)
		{
			if (isShow)
			{
				List<string[]> list = ModelManager.Instance.Get_LoginList();
				if (list == null || list.Count == 0 || this.isCancelAccount)
				{
					this.SpeedRegister.gameObject.SetActive(true);
					this.ListLogin.gameObject.SetActive(false);
					this.Message.value = string.Empty;
					this.panel3.gameObject.SetActive(false);
					this.panel2.gameObject.SetActive(false);
					this.panel1.gameObject.SetActive(true);
				}
				else
				{
					string text = ModelManager.Instance.Get_LoginList()[0][0];
					string text2 = ModelManager.Instance.Get_LoginList()[0][1];
					if (text != null && text2 != null && text != string.Empty && text2 != string.Empty)
					{
						this.gameObject.SetActive(false);
						this.LoginServer(text, text2, 2);
					}
				}
			}
			else
			{
				this.Bg.gameObject.SetActive(false);
				this.panel2.gameObject.SetActive(false);
				this.panel1.gameObject.SetActive(false);
			}
		}

		public void SDKLoginFail()
		{
			UIEventListener.Get(this.mask.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickMask);
			this.mask.gameObject.SetActive(true);
			this.WarnText.gameObject.SetActive(true);
			if (GlobalSettings.isLoginByAnySDK && InitAnySDK.getInstance().anySDKUser.isLogined() && Singleton<LoginView_New>.Instance.gameObject != null && Singleton<LoginView_New>.Instance.gameObject.active)
			{
				this.mask.gameObject.SetActive(false);
				this.coroutineManager.StartCoroutine(InitSDK.instance.TryLoginByChannelId(AnySDK.getInstance().getChannelId(), InitAnySDK.getInstance().anySDKUser.getUserID()), true);
			}
		}

		private void OnClickMask(GameObject obj)
		{
			if (GlobalSettings.isLoginByHoolaiSDK)
			{
				InitSDK.instance.StartSDKLogin();
			}
			else if (GlobalSettings.isLoginByAnySDK)
			{
				this.mask.gameObject.SetActive(false);
				InitSDK.instance.StartAnySDKLogin();
			}
			if (Application.isEditor)
			{
				this.panel1.gameObject.SetActive(true);
				this.Bg.gameObject.SetActive(true);
				this.WarnText.gameObject.SetActive(false);
				this.mask.gameObject.SetActive(false);
			}
		}

		public void HideLoginMask()
		{
			if (this.transform != null)
			{
				this.mask.gameObject.SetActive(false);
			}
		}

		public void ShowLoginMask()
		{
			if (this.transform != null)
			{
				this.mask.gameObject.SetActive(true);
			}
		}

		private void OnToLogin(GameObject obj)
		{
			this.isForgetMode = false;
			this.panel3.gameObject.SetActive(false);
			this.panel2.gameObject.SetActive(true);
			this.warn.gameObject.SetActive(false);
			this.lb_enterGame.text = "进入游戏";
		}

		private void OnToRegistration(GameObject obj)
		{
			this.Message.value = string.Empty;
			this.panel3.gameObject.SetActive(true);
			this.panel2.gameObject.SetActive(false);
			this.ShowSetPassWord(false);
			this.ValidateLabel.text = "点击注册";
			this.warn.gameObject.SetActive(false);
			this.CheckPhoneNumber(this.isCodeTimerRun);
			this.GetMessage.gameObject.SetActive(true);
			this.GetMessage1.gameObject.SetActive(false);
		}

		private void OnForget(GameObject obj)
		{
			this.isForgetMode = true;
			this.Message.value = string.Empty;
			this.panel3.gameObject.SetActive(true);
			this.panel2.gameObject.SetActive(false);
			this.ShowSetPassWord(false);
			this.ValidateLabel.text = "修改密码";
			this.warn.gameObject.SetActive(false);
			this.CheckPhoneNumber(this.isChangeCodeTimerRun);
			this.GetMessage.gameObject.SetActive(false);
			this.GetMessage1.gameObject.SetActive(true);
		}

		private void CheckPhoneNumber()
		{
			bool isTimerRun;
			if (this.ValidateLabel.text == "修改密码")
			{
				isTimerRun = this.isChangeCodeTimerRun;
			}
			else
			{
				isTimerRun = this.isCodeTimerRun;
			}
			this.CheckPhoneNumber(isTimerRun);
		}

		private void CheckPhoneNumber(bool _isTimerRun)
		{
			UIButton uIButton;
			UILabel uILabel;
			if (this.ValidateLabel.text == "修改密码")
			{
				uIButton = this.GetMessage1;
				uILabel = this.GetMessageLabel1;
			}
			else
			{
				uIButton = this.GetMessage;
				uILabel = this.GetMessageLabel;
			}
			if (this.YourUserName.value.Length == 11 && !_isTimerRun)
			{
				uIButton.state = UIButtonColor.State.Normal;
				uILabel.gradientTop = Color.white;
				uILabel.gradientBottom = new Color(0.329411775f, 0.8156863f, 1f);
				uIButton.collider.enabled = true;
			}
			else
			{
				uIButton.state = UIButtonColor.State.Disabled;
				uILabel.gradientTop = Color.white;
				uILabel.gradientBottom = new Color(0.827451f, 0.827451f, 0.827451f);
				uIButton.collider.enabled = false;
			}
		}

		private void TryGetMessage(GameObject obj)
		{
			if (this.YourUserName.value.Length == 11)
			{
				if (this.isForgetMode)
				{
					this.FindMyAccountPasswd(obj);
				}
				else
				{
					SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(false, "正在获得验证码...", true, 15f);
					SendMsgManager.Instance.SendMsg(MobaMasterCode.GetPhoneCode, param, new object[]
					{
						this.YourUserName.value
					});
				}
			}
			else
			{
				this.ShowWarn(3);
			}
		}

		private void CheckPhoneAndMessage(GameObject obj)
		{
			if (this.GetMessageLabel.text == string.Empty)
			{
				this.ShowWarn(4);
				return;
			}
			if (this.YourUserName.value.Length == 11 && this.GetMessageLabel.text != string.Empty)
			{
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(false, "正在验证...", true, 15f);
				SendMsgManager.Instance.SendMsg(MobaMasterCode.CheckPhoneAndCode, param, new object[]
				{
					this.YourUserName.value,
					this.Message.label.text
				});
			}
		}

		private void FindMyAccountPasswd(GameObject obj)
		{
			if (this.YourUserName.value.Length == 11 && this.GetMessageLabel.text != string.Empty)
			{
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(false, "请求找回密码...", true, 15f);
				SendMsgManager.Instance.SendMsg(MobaMasterCode.FindMyAccountPasswd, param, new object[]
				{
					this.YourUserName.value
				});
			}
		}

		private void OnQQIcon(GameObject objct_1 = null)
		{
			InitSDK.instance.LoginByQQ();
		}

		private void OnWeChat(GameObject objct_1 = null)
		{
			InitSDK.instance.LoginByWeChat();
		}

		private void clickSpeedRegister(GameObject objct_1 = null)
		{
			string deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
			this.coroutineManager.StartCoroutine(InitSDK.instance.TryLoginByChannelId("Visitor", deviceUniqueIdentifier), true);
		}

		private void clickRegistration(GameObject objct_1 = null)
		{
			if (this.YourUserNameLabel.text == string.Empty || this.YourUserNameLabel.text == "请输入预约的手机号" || this.YourUserNameLabel.text.Length < 6)
			{
				this.ShowSetPassWord(false);
				CtrlManager.ShowMsgBox("注册失败", "请输入一个6-12位数字或字母组成的账号", new Action(this.RegistNameIsWrong), PopViewType.PopOneButton, "确定", "取消", null);
			}
			else if (string.IsNullOrEmpty(this.RegistrationPIN.value) || this.RegistrationPIN.value.Length < 6)
			{
				CtrlManager.ShowMsgBox("注册失败", "请输入一个6-12位数字或字母组成的密码", new Action(this.RegistPasswordIsWrong1), PopViewType.PopOneButton, "确定", "取消", null);
				this.ShowWarn(5);
			}
			else if (this.RegistrationPIN.value != this.PINAgain.value)
			{
				CtrlManager.ShowMsgBox("注册失败", "请保证两次输入的密码完全相同", new Action(this.RegistPasswordIsWrong2), PopViewType.PopOneButton, "确定", "取消", null);
				this.ShowWarn(6);
			}
			else
			{
				string text = this.YourUserNameLabel.text;
				string value = this.RegistrationPIN.value;
				if (!this.isForgetMode)
				{
					this.LoginServer(text, value, 3);
				}
				else
				{
					SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(false, "正在修改密码...", true, 15f);
					SendMsgManager.Instance.SendMsg(MobaMasterCode.ModifyAccountPasswd, param, new object[]
					{
						text,
						this.RegistrationPIN.value
					});
				}
			}
		}

		private void RegistNameIsWrong()
		{
			if (this.YourUserName != null)
			{
				this.YourUserName.value = string.Empty;
			}
		}

		private void RegistPasswordIsWrong1()
		{
			if (this.RegistrationPIN != null)
			{
				this.RegistrationPIN.value = string.Empty;
			}
		}

		private void RegistPasswordIsWrong2()
		{
			if (this.PINAgain != null)
			{
				this.PINAgain.value = string.Empty;
			}
		}

		public void ShowOtherLogin()
		{
			this.panel1.gameObject.SetActive(false);
			this.panel2.gameObject.SetActive(true);
			this.BackButton.gameObject.SetActive(true);
			this.record = backButton.panel2;
		}

		private void clickRegisterAccount(GameObject objct_1 = null)
		{
			List<string[]> list = ModelManager.Instance.Get_LoginList();
			if (list.Count >= 1)
			{
				long num = 0L;
				string text = ModelManager.Instance.Get_LoginList()[0][0];
				string text2 = ModelManager.Instance.Get_LoginList()[0][1];
				if (!long.TryParse(text, out num) && list.Count > 1)
				{
					text = ModelManager.Instance.Get_LoginList()[1][0];
					text2 = ModelManager.Instance.Get_LoginList()[1][1];
				}
				if (text != null && text2 != null && text != string.Empty && text2 != string.Empty)
				{
					this.panel1.gameObject.SetActive(false);
					this.Message.value = string.Empty;
					this.panel3.gameObject.SetActive(false);
					this.panel2.gameObject.SetActive(true);
					this.BackButton.gameObject.SetActive(true);
					this.SNS.gameObject.SetActive(false);
					this.record = backButton.panel2;
					this.Account.value = text;
					this.PIN.value = text2;
				}
			}
			else
			{
				this.panel1.gameObject.SetActive(false);
				this.Message.value = string.Empty;
				this.panel3.gameObject.SetActive(true);
				this.panel2.gameObject.SetActive(false);
				this.BackButton.gameObject.SetActive(true);
				this.SNS.gameObject.SetActive(false);
				this.record = backButton.panel3;
			}
		}

		private void LoginGame(GameObject objct_1 = null)
		{
			if (string.IsNullOrEmpty(this.Account.value) || (this.Account.value.Length != 11 && !Application.isEditor))
			{
				this.ShowWarn(3);
				CtrlManager.ShowMsgBox((!(LanguageManager.Instance.GetStringById("LoginUI_Title_LoginError") != string.Empty)) ? "登录错误" : LanguageManager.Instance.GetStringById("LoginUI_Title_LoginError"), (!(LanguageManager.Instance.GetStringById("LoginUI_Content_LoginError1") != string.Empty)) ? "请输入11位的手机号" : LanguageManager.Instance.GetStringById("LoginUI_Content_LoginError1"), new Action(this.LoginNameIsWrong), PopViewType.PopOneButton, "确定", "取消", null);
			}
			else if ((this.Account.value != null && string.IsNullOrEmpty(this.PIN.value)) || this.PIN.value.Length < 6)
			{
				CtrlManager.ShowMsgBox((!(LanguageManager.Instance.GetStringById("LoginUI_Title_LoginError") != string.Empty)) ? "登录错误" : LanguageManager.Instance.GetStringById("LoginUI_Title_LoginError"), (!(LanguageManager.Instance.GetStringById("LoginUI_Content_LoginError2") != string.Empty)) ? "请输入和账号对应的6-12位密码" : LanguageManager.Instance.GetStringById("LoginUI_Content_LoginError2"), new Action(this.LoginPINIsWrong), PopViewType.PopOneButton, "确定", "取消", null);
			}
			else if (this.Account.value != null && this.PIN.value != null && this.Account.value != string.Empty && this.PIN.value != string.Empty)
			{
				this.LoginServer(this.Account.value, this.PIN.value, 2);
			}
		}

		public void LoginNameIsWrong()
		{
			this.Account.isSelected = true;
		}

		public void LoginPINIsWrong()
		{
			this.PIN.value = string.Empty;
			this.PIN.isSelected = true;
		}

		public void LoginNameOrPINIsWrong()
		{
			this.PIN.value = string.Empty;
			this.PIN.isSelected = false;
			this.Account.isSelected = true;
		}

		private void ClickBackButton(GameObject objct_1 = null)
		{
			backButton backButton = this.record;
			if (backButton != backButton.panel2)
			{
				if (backButton == backButton.panel3)
				{
					this.panel1.gameObject.SetActive(true);
					this.BackButton.gameObject.SetActive(false);
					this.panel3.gameObject.SetActive(false);
					this.panel2.gameObject.SetActive(false);
					this.SNS.gameObject.SetActive(false);
					this.record = backButton.panel1;
				}
			}
			else
			{
				this.panel1.gameObject.SetActive(true);
				this.BackButton.gameObject.SetActive(false);
				this.panel2.gameObject.SetActive(false);
				this.panel3.gameObject.SetActive(false);
				this.SNS.gameObject.SetActive(false);
				this.record = backButton.panel1;
			}
		}

		private void clickUseAccountLogin(GameObject objct_1 = null)
		{
			if (this.SpeedRegister.gameObject.activeInHierarchy)
			{
				this.panel1.gameObject.SetActive(false);
				this.panel2.gameObject.SetActive(true);
				this.BackButton.gameObject.SetActive(true);
				this.record = backButton.panel2;
			}
			else
			{
				string editpassworld = string.Empty;
				List<string[]> list = this.ListLogin.GetComponent<UIListLogin>().userData;
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i][0] == this.ListLogin.GetComponent<UIListLogin>().Account)
					{
						editpassworld = list[i][1];
						break;
					}
				}
				this.LoginServer(this.ListLogin.GetComponent<UIListLogin>().Account, editpassworld, 2);
			}
		}

		private void LoginServer(string editusername, string editpassworld, byte LoginType)
		{
			AccountData accountData = new AccountData
			{
				Mail = editusername,
				UserName = editusername,
				Password = editpassworld,
				ChannelId = "1"
			};
			ModelManager.Instance.Set_accountData_X(accountData);
			if (this.toggle.value)
			{
				PlayerPrefs.SetString("SavePassWord", "true");
			}
			else
			{
				PlayerPrefs.SetString("SavePassWord", "false");
			}
			if (LoginType == 1)
			{
				MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)21006, accountData, 0f);
				MobaMessageManager.ExecuteMsg(message);
			}
			else if (LoginType == 2)
			{
				MobaMessage message2 = MobaMessageManager.GetMessage((ClientMsg)21004, accountData, 0f);
				MobaMessageManager.ExecuteMsg(message2);
			}
			else if (LoginType == 3)
			{
				MobaMessage message3 = MobaMessageManager.GetMessage((ClientMsg)21005, accountData, 0f);
				MobaMessageManager.ExecuteMsg(message3);
			}
		}

		private void OnRegister(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				if (mobaErrorCode == MobaErrorCode.UserExist)
				{
					this.LoginNameIsWrong();
				}
			}
			else
			{
				this.warn.gameObject.SetActive(false);
			}
		}

		private void OnGuestUpgrade(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				if (mobaErrorCode == MobaErrorCode.UserExist)
				{
					this.LoginNameIsWrong();
				}
			}
			else
			{
				this.warn.gameObject.SetActive(false);
			}
		}

		private void OnGetPhoneCode(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				if (mobaErrorCode == MobaErrorCode.PhoneGetCodeError)
				{
					this.ShowWarn(1);
				}
			}
			else
			{
				this.isCodeTimerRun = true;
				this.CodeTimerValue = 60;
				this.coroutineManager.StartCoroutine(this.CodeTimer(), true);
				this.CheckPhoneNumber();
			}
		}

		private void OnCheckPhoneAndCode(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.PhoneGetCodeError)
			{
				if (mobaErrorCode != MobaErrorCode.PhoneCodeError)
				{
					if (mobaErrorCode == MobaErrorCode.Ok)
					{
						this.ShowSetPassWord(true);
					}
				}
				else
				{
					this.ShowWarn(2);
				}
			}
			else
			{
				this.ShowWarn(2);
			}
		}

		private void OnFindMyAccountPasswd(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				this.ShowWarn(1);
			}
			else
			{
				this.isChangeCodeTimerRun = true;
				this.ChangeCodeTimerValue = 60;
				this.coroutineManager.StartCoroutine(this.ChangeCodeTimer(), true);
				this.CheckPhoneNumber();
			}
		}

		private void OnModifyAccountPasswd(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				if (mobaErrorCode != MobaErrorCode.FindAccountPasswdError)
				{
				}
			}
			else
			{
				this.isForgetMode = false;
				string text = this.YourUserNameLabel.text;
				string value = this.RegistrationPIN.value;
				this.LoginServer(text, value, 2);
			}
		}

		private void ShowSetPassWord(bool isShow = true)
		{
			if (isShow)
			{
				this.YourUserName.gameObject.SetActive(false);
				this.Message.gameObject.SetActive(false);
				this.GetMessage.gameObject.SetActive(false);
				this.RegistrationPIN.gameObject.SetActive(true);
				this.PINAgain.gameObject.SetActive(true);
				this.Validate.gameObject.SetActive(false);
				this.Registration.gameObject.SetActive(true);
				AutoTestController.InvokeTestLogic(AutoTestTag.Login, delegate
				{
					this.RegistrationPIN.value = "aaaaaa";
					this.PINAgain.value = this.RegistrationPIN.value;
					this.clickRegistration(null);
				}, 1f);
			}
			else
			{
				this.YourUserName.gameObject.SetActive(true);
				this.Message.gameObject.SetActive(true);
				this.GetMessage.gameObject.SetActive(true);
				this.RegistrationPIN.gameObject.SetActive(false);
				this.PINAgain.gameObject.SetActive(false);
				this.Validate.gameObject.SetActive(true);
				this.Registration.gameObject.SetActive(false);
			}
		}

		private void ShowWarn(int type)
		{
			this.warn.gameObject.SetActive(true);
			switch (type)
			{
			case 1:
				this.warn.transform.localPosition = new Vector3(-297.1f, 133.7f, 0f);
				this.warnTitle.text = "该手机号未注册";
				this.warnLabel.text = "请输入您注册的手机号";
				break;
			case 2:
				this.warn.transform.localPosition = new Vector3(-297.1f, -9.209999f, 0f);
				this.warnTitle.text = "验证码不正确";
				this.warnLabel.text = "请输入您短信收到的验证码";
				break;
			case 3:
				this.warn.transform.localPosition = new Vector3(-297.1f, 133.7f, 0f);
				this.warnTitle.text = "请输入手机号";
				this.warnLabel.text = "请输入您预约时的手机号";
				break;
			case 4:
				this.warn.transform.localPosition = new Vector3(-297.1f, -9.209999f, 0f);
				this.warnTitle.text = "请输入验证码";
				this.warnLabel.text = "请输入您预约的手机号";
				break;
			case 5:
				this.warn.transform.localPosition = new Vector3(-297.1f, 133.7f, 0f);
				this.warnTitle.text = "密码格式不正确";
				this.warnLabel.text = "格式为6-16位的字母和数字";
				break;
			case 6:
				this.warn.transform.localPosition = new Vector3(-297.1f, -9.209999f, 0f);
				this.warnTitle.text = "两次输入的密码不一致";
				this.warnLabel.text = "               请输入正确的密码";
				break;
			case 7:
				this.warn.transform.localPosition = new Vector3(-297.1f, -9.209999f, 0f);
				this.warnTitle.text = "密码不正确";
				this.warnLabel.text = "你可点击“忘记密码？”找回密码";
				break;
			}
		}

		[DebuggerHidden]
		private IEnumerator CodeTimer()
		{
			LoginView_New.<CodeTimer>c__Iterator163 <CodeTimer>c__Iterator = new LoginView_New.<CodeTimer>c__Iterator163();
			<CodeTimer>c__Iterator.<>f__this = this;
			return <CodeTimer>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator ChangeCodeTimer()
		{
			LoginView_New.<ChangeCodeTimer>c__Iterator164 <ChangeCodeTimer>c__Iterator = new LoginView_New.<ChangeCodeTimer>c__Iterator164();
			<ChangeCodeTimer>c__Iterator.<>f__this = this;
			return <ChangeCodeTimer>c__Iterator;
		}

		private void OnLogin(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.InvalidOperation)
			{
				if (mobaErrorCode != MobaErrorCode.InvalidParameter)
				{
					if (mobaErrorCode != MobaErrorCode.Ok)
					{
						if (mobaErrorCode == MobaErrorCode.UserNotExist)
						{
							this.gameObject.SetActive(true);
							this.LoginNameIsWrong();
						}
					}
					else
					{
						this.warn.gameObject.SetActive(false);
					}
				}
			}
			else
			{
				this.gameObject.SetActive(true);
				this.LoginNameOrPINIsWrong();
				this.ShowWarn(7);
			}
		}

		private void OnLoginByPlatformUid(MobaMessage msg)
		{
			if (Singleton<LoginView_New>.Instance.gameObject != null && Singleton<LoginView_New>.Instance.gameObject.active)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				int num = (int)operationResponse.Parameters[1];
				if (GlobalSettings.isLoginByHoolaiSDK)
				{
					this.mask.gameObject.SetActive(true);
				}
				MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
				if (mobaErrorCode == MobaErrorCode.Ok)
				{
					Dictionary<byte, object> dictionary = operationResponse.Parameters[85] as Dictionary<byte, object>;
					AccountData accountData;
					if (dictionary == null)
					{
						accountData = SerializeHelper.Deserialize<AccountData>(operationResponse.Parameters[85] as byte[]);
					}
					else
					{
						accountData = this.ToAccountData(dictionary);
					}
					if (accountData == null)
					{
						Singleton<TipView>.Instance.ShowViewSetText("账号数据为空！！", 1f);
					}
					else
					{
						Singleton<TipView>.Instance.ShowViewSetText("正在登录", 1f);
					}
					this.LoginServer(accountData.UserName, accountData.Password, 2);
					if (GlobalSettings.isLoginByAnySDK)
					{
						InitAnySDK.getInstance().showToolBar(ToolBarPlace.kToolBarBottomLeft);
					}
				}
				return;
			}
		}

		private AccountData ToAccountData(object serObj)
		{
			if (serObj is IDictionary)
			{
				Dictionary<byte, object> dictionary = (Dictionary<byte, object>)serObj;
				AccountData accountData = new AccountData();
				accountData.AccountId = (string)dictionary[71];
				accountData.UserName = (string)dictionary[72];
				accountData.Password = (string)dictionary[74];
				accountData.Mail = (string)dictionary[73];
				accountData.UserType = (int)dictionary[77];
				accountData.DeviceType = (int)dictionary[75];
				accountData.DeviceToken = (string)dictionary[76];
				accountData.ServerName = (int)dictionary[53];
				accountData.ChannelId = (string)dictionary[78];
				if (dictionary.ContainsKey(225))
				{
					accountData.AccessToken = (string)dictionary[225];
				}
				if (dictionary.ContainsKey(222))
				{
					accountData.PlatformUid = (string)dictionary[222];
				}
				if (dictionary.ContainsKey(223))
				{
					accountData.Channel = (string)dictionary[223];
				}
				if (dictionary.ContainsKey(224))
				{
					accountData.ChannelUid = (string)dictionary[224];
				}
				if (dictionary.ContainsKey(227))
				{
					accountData.IsBindPhone = (bool)dictionary[227];
				}
				if (dictionary.ContainsKey(228))
				{
					accountData.IsBindEmail = (bool)dictionary[228];
				}
				return accountData;
			}
			return null;
		}

		public void TestRegisterAgain()
		{
			if (Singleton<LoginView_New>.Instance.Account)
			{
				string randomName = this.GetRandomName();
				UIInput arg_34_0 = Singleton<LoginView_New>.Instance.Account;
				string value = randomName;
				this.YourUserName.value = value;
				arg_34_0.value = value;
				AutoTestController.InvokeTestLogic(AutoTestTag.Login, delegate
				{
					this.RegistrationPIN.value = "aaaaaa";
					this.PINAgain.value = this.RegistrationPIN.value;
					this.clickRegistration(null);
				}, 1f);
			}
		}

		private string GetRandomName()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < 6; i++)
			{
				char value = (char)(97 + UnityEngine.Random.Range(0, 25));
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}
	}
}
