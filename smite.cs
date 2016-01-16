usando Sistema;
usando System.Drawing;
usando System.Linq;
usando EloBuddy;
usando EloBuddy.SDK;
usando EloBuddy.SDK.Menu.Values;
usando EloBuddy.SDK.Rendering;

namespace  Shaco
{
    interno  classe  Smite
    {
        público  estático  dupla TotalDamage = 0;
        público  estático AIHeroClient Jogador
        {
            obter {retornar ObjectManager.Player; }
        }

        público  estático Spell.Targeted SmiteSpell;

        público  estático Monstro Obj_AI_Base;

        público  estático Texto SmiteStatus = novo texto ("",
            nova Fonte (FontFamily.GenericSansSerif, 9, FontStyle.Bold));

        público  estático  readonly  corda [] = SmiteableUnits
        {
            "SRU_Red", "SRU_Blue", "SRU_Dragon", "SRU_Baron",
            "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak",
            "SRU_Krug", "Sru_Crab"
        };

        privado  estático  readonly  int [] SmiteRed = {3715, 1415, 1414, 1413, 1412};
        privado  estático  readonly  int [] SmiteBlue = {3706, 1403, 1402, 1401, 1400};

        público  estático  vazio  Smitemethod ()
        {
            ThugDogeShaco.SmiteMenu = ThugDogeShaco.ShacoMenu.AddSubMenu ("Smite", "Smite");
            ThugDogeShaco.SmiteMenu.AddSeparator ();
            ThugDogeShaco.SmiteMenu.Add ("smiteActive",
                nova keybind ("Fere Ativa (alternar)", verdadeiro, KeyBind.BindTypes.PressToggle, 'H'));
            ThugDogeShaco.SmiteMenu.AddSeparator ();
            ThugDogeShaco.SmiteMenu.Add ("useSlowSmite", novo CheckBox ("KS azul com Smite"));
            ThugDogeShaco.SmiteMenu.Add ("comboWithDuelSmite", novo CheckBox ("Combo com a Red Smite"));
            ThugDogeShaco.SmiteMenu.AddSeparator ();
            ThugDogeShaco.SmiteMenu.AddGroupLabel ("Camps");
            ThugDogeShaco.SmiteMenu.AddLabel ("épicos");
            ThugDogeShaco.SmiteMenu.Add ("SRU_Baron", novo CheckBox ("Baron"));
            ThugDogeShaco.SmiteMenu.Add ("SRU_Dragon", novo CheckBox ("dragão"));
            ThugDogeShaco.SmiteMenu.AddLabel ("Os aficionados por");
            ThugDogeShaco.SmiteMenu.Add ("SRU_Blue", novo CheckBox ("azul"));
            ThugDogeShaco.SmiteMenu.Add ("SRU_Red", novo CheckBox ("Red"));
            ThugDogeShaco.SmiteMenu.AddLabel ("Pequenos Camps");
            ThugDogeShaco.SmiteMenu.Add ("SRU_Gromp", novo CheckBox ("Gromp", false));
            ThugDogeShaco.SmiteMenu.Add ("SRU_Murkwolf", novo CheckBox ("Murkwolf", false));
            ThugDogeShaco.SmiteMenu.Add ("SRU_Krug", novo CheckBox ("Krug", false));
            ThugDogeShaco.SmiteMenu.Add ("SRU_Razorbeak", novo CheckBox ("Razerbeak", false));
            ThugDogeShaco.SmiteMenu.Add ("Sru_Crab", novo CheckBox ("Skuttles", false));

            Game.OnUpdate + = SmiteEvent;
        }

        público  estático  vazio  SetSmiteSlot ()
        {
            SpellSlot smiteSlot;
            se (SmiteBlue.Any (x => Player.InventoryItems.FirstOrDefault (a => a.Id == (ItemId) x)! = NULL))
                smiteSlot = Player.GetSpellSlotFromName ("s5_summonersmiteplayerganker");
            outra coisa  se ​​(
                SmiteRed.Any (
                    x => Player.InventoryItems.FirstOrDefault (a => a.Id == (ItemId) x)! = NULL))
                smiteSlot = Player.GetSpellSlotFromName ("s5_summonersmiteduel");
            outro
                smiteSlot = Player.GetSpellSlotFromName ("summonersmite");
            SmiteSpell = new Spell.Targeted (smiteSlot, 500);
        }

        público  static  int  GetSmiteDamage ()
        {
            var nível = Player.Level;
            int [] = smitedamage
            {
                20 * nível + 370,
                30 * nível + 330,
                40 * nível + 240,
                50 * nível + 100
            };
            retornar smitedamage.Max ();
        }

        privado  estático  vazio  SmiteEvent (EventArgs  args)
        {
            SetSmiteSlot ();
            se (SmiteSpell.IsReady () || Player.IsDead!) retorno;
            se (ThugDogeShaco.SmiteMenu ["smiteActive"] .Cast <keybind> (). CurrentValue)
            {
                var unidade =
                    EntityManager.MinionsAndMonsters.Monsters
                        .Onde(
                            A =>
                                SmiteableUnits.Contains (a.BaseSkinName) && a.Health <GetSmiteDamage () &&
                                ThugDogeShaco.SmiteMenu [a.BaseSkinName] .Cast <CheckBox> (). CurrentValue)
                        .OrderByDescending (A => a.MaxHealth)
                        .FirstOrDefault ();

                se (unidade! = NULL)
                {
                    SmiteSpell.Cast (unidade);
                    retornar;
                }
            }
            se (ThugDogeShaco.SmiteMenu ["useSlowSmite"] .Cast <CheckBox> (). CurrentValue &&
                SmiteSpell.Handle.Name == "s5_summonersmiteplayerganker")
            {
                foreach (
                    var alvo em
                        EntityManager.Heroes.Enemies
                            .onde (H => h.IsValidTarget (SmiteSpell.Range) && h.Health <= 20 + 8 * Player.Level))
                {
                    SmiteSpell.Cast (alvo);
                    retornar;
                }
            }
            se (ThugDogeShaco.SmiteMenu ["comboWithDuelSmite"] .Cast <CheckBox> (). CurrentValue &&
                SmiteSpell.Handle.Name == "s5_summonersmiteduel" &&
                Orbwalker.ActiveModesFlags.HasFlag (Orbwalker.ActiveModes.Combo))
            {
                foreach (
                    var alvo em
                        EntityManager.Heroes.Enemies
                            .onde (H => h.IsValidTarget (SmiteSpell.Range)). OrderByDescending (TargetSelector.GetPriority)
                    )
                {
                    SmiteSpell.Cast (alvo);
                    retornar;
                }
            }
        }
    }
}
