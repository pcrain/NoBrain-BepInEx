# Wiki Regex

https://discordapp.com/channels/212584696410800130/223436959840862208/479019782105071632


# other stuff...
In Ammonomecon?
And when buying stuff? ShopItemController#	public void OnEnteredRange(PlayerController interactor)


		text2 = string.Concat(new object[]
		{
			text2,
			" ",
			this.item.PickupObjectId,
			" ",
			this.item.quality,
			" ",
			this.item.contentSource,
			"\n",
			component.journalData.GetAmmonomiconFullEntry(false, false)
		});
		text2 += "\nType: Active\nStuns all enemies\nCan be used to steal items from the shop";
		GameObject gameObject = GameUIRoot.Instance.RegisterDefaultLabel(base.transform, offset, text2);
		dfLabel componentInChildren = gameObject.GetComponentInChildren<dfLabel>();
		componentInChildren.ColorizeSymbols = false;
		componentInChildren.ProcessMarkup = true;
		componentInChildren.AutoHeight = true;
		componentInChildren.WordWrap = true;
		componentInChildren.TextScale = 2f;
		componentInChildren.Width = (float)(Screen.width / 4);
		gameObject.GetComponentInChildren<dfPanel>().FitToContents();
		
## UPDATE

		bool flag = !(this.item is AmmoPickup) && !(this.item is HealthPickup) && !(this.item is KeyBulletPickup);
		if (flag)
		{
			text2 = string.Concat(new object[]
			{
				text2,
				" ",
				this.item.PickupObjectId,
				" ",
				this.item.quality,
				" ",
				(this.item is PassiveItem) ? "Passive" : "Active"
			});
			if (component != null)
			{
				text2 = string.Concat(new object[]
				{
					text2,
					"\n",
					component.journalData.GetAmmonomiconFullEntry(false, false)
				});
			}
		}
		GameObject gameObject = GameUIRoot.Instance.RegisterDefaultLabel(base.transform, offset, text2);
		dfLabel componentInChildren = gameObject.GetComponentInChildren<dfLabel>();
		componentInChildren.ColorizeSymbols = false;
		componentInChildren.ProcessMarkup = true;
		if (flag)
		{
			componentInChildren.TextScale = 2f;
			componentInChildren.AutoHeight = true;
			componentInChildren.WordWrap = true;
			componentInChildren.Width = (float)(Screen.width / 4);
			gameObject.GetComponentInChildren<dfPanel>().FitToContents();
		}

## WHEN RELOAD WAS PRESSED:

		interactor.OnReloadPressed += 

## Dont describe Ammo
class AmmoPickup

## NOT WORKING

				foreach (PickupObject o in PickupObjectDatabase.m_instance.Objects)
				{
					ETGModConsole.Log(o.PickupObjectId + " " + o.DisplayName, false);
				}

[sprite \"armor_money_icon_001\"][sprite \"escape_rope_text_icon_001\"][sprite \"forged_bullet_case_001\"][sprite \"forged_bullet_powder_001\"][sprite \"forged_bullet_primer_001\"][sprite \"forged_bullet_slug_001\"][sprite \"hbux_text_icon\"][sprite \"heart_big_idle_001\"][sprite \"master_token_icon_001\"][sprite \"master_token_icon_002\"][sprite \"master_token_icon_003\"][sprite \"master_token_icon_004\"][sprite \"master_token_icon_005\"][sprite \"poopsack_001\"][sprite \"resourceful_rat_icon_001\"][sprite \"teleport_active_001\"][sprite \"ui_blank\"][sprite \"ui_coin\"][sprite \"ui_key\"]

Others:
newkey_idle_001


Controller icons:

armor_money_icon_001
escape_rope_text_icon_001
forged_bullet_case_001
forged_bullet_powder_001
forged_bullet_primer_001
forged_bullet_slug_001
hbux_text_icon
heart_big_idle_001
master_token_icon_001
master_token_icon_002
master_token_icon_003
master_token_icon_004
master_token_icon_005
poopsack_001
resourceful_rat_icon_001
teleport_active_001
ui_blank
ui_coin
ui_key

ps4_circle
ps4_cross
ps4_dpad
ps4_dpad_down
ps4_dpad_left
ps4_dpad_right
ps4_dpad_up
ps4_flat
ps4_L1
ps4_L2
ps4_L3
ps4_LS
ps4_options_share
ps4_R1
ps4_R2
ps4_R3
ps4_RS
ps4_RS_down
ps4_RS_left
ps4_RS_right
ps4_RS_up
ps4_square
ps4_triangle
xbone_a
xbone_b
xbone_dpad
xbone_dpad_down
xbone_dpad_left
xbone_dpad_right
xbone_dpad_up
xbone_L3
xbone_LB
xbone_LS
xbone_LT
xbone_R3
xbone_RB
xbone_RS
xbone_RT
xbone_select
xbone_start
xbone_x
xbone_y