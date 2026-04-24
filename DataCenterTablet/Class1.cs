using MelonLoader;
using UnityEngine;
using Il2Cpp;
using UnityEngine.InputSystem;
using System.Collections;

[assembly: MelonInfo(typeof(DataCenterTablet.Main), "Data Center Logistics Tablet", "0.9.0", "Lzinsp")]
[assembly: MelonGame(null, null)]

namespace DataCenterTablet
{
    public class Main : MelonMod
    {
        private bool exibirTablet = false;
        private float red = 1f, green = 0f, blue = 0f;
        private Color CustomColor => new Color(red, green, blue, 1f);

        public override void OnUpdate()
        {
            // T Key - Toggle UI
            if (Keyboard.current.tKey.wasPressedThisFrame)
            {
                exibirTablet = !exibirTablet;
                Cursor.visible = exibirTablet;
                Cursor.lockState = exibirTablet ? CursorLockMode.None : CursorLockMode.Locked;
            }

            if (exibirTablet)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Input.ResetInputAxes(); // Prevents camera jitter while using tablet
            }
        }

        public override void OnGUI()
        {
            if (!exibirTablet) return;

            // Force cursor visibility over GUI layer
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // Main UI Window
            Rect win = new Rect((Screen.width - 780) / 2, (Screen.height - 820) / 2, 780, 820);
            GUI.Box(win, "<b><size=20> DATA CENTER LOGISTICS TABLET </size></b>");

            // --- 1. CUSTOM CABLING ---
            GUI.Label(new Rect(win.x + 20, win.y + 45, 300, 20), "<b>1. CUSTOM CAT6E CABLING (RGB)</b>");
            red = GUI.HorizontalSlider(new Rect(win.x + 40, win.y + 70, 150, 20), red, 0f, 1f);
            green = GUI.HorizontalSlider(new Rect(win.x + 40, win.y + 90, 150, 20), green, 0f, 1f);
            blue = GUI.HorizontalSlider(new Rect(win.x + 40, win.y + 110, 150, 20), blue, 0f, 1f);

            Color oldC = GUI.color; GUI.color = CustomColor;
            GUI.DrawTexture(new Rect(win.x + 205, win.y + 70, 80, 60), Texture2D.whiteTexture);
            GUI.color = oldC; GUI.Box(new Rect(win.x + 205, win.y + 70, 80, 60), "");

            if (GUI.Button(new Rect(win.x + 300, win.y + 70, 460, 60), "<b>GENERATE CUSTOM CABLE ($500)</b>"))
                PurchaseItem(2, 500, PlayerManager.ObjectInHand.CableSpinner, "Cat6E Custom", CustomColor);

            // --- 2. SERVERS ---
            GUI.Label(new Rect(win.x + 20, win.y + 145, 500, 20), "<b>2. COMPUTING UNITS (SERVERS)</b>");
            DrawHardwareRow(win, 170, "System X 3U", 0, "System X 7U", 1, 1500, 2800, PlayerManager.ObjectInHand.Server1U);
            DrawHardwareRow(win, 215, "RISC 3U", 2, "RISC 7U", 3, 3500, 6200, PlayerManager.ObjectInHand.Server1U);
            DrawHardwareRow(win, 260, "GPU Server", 4, "GPU Ultra", 5, 12000, 25000, PlayerManager.ObjectInHand.Server3U);
            DrawHardwareRow(win, 305, "Mainframe", 6, "Mainframe Pro", 7, 8500, 14000, PlayerManager.ObjectInHand.Server2U);

            // --- 3. NETWORKING ---
            GUI.Label(new Rect(win.x + 20, win.y + 360, 500, 20), "<b>3. NETWORK SWITCHES</b>");
            DrawHardwareRow(win, 385, "16x 10Gbps RJ45", 0, "4x SFP+/SFP28", 1, 250, 400, PlayerManager.ObjectInHand.Switch);
            DrawHardwareRow(win, 430, "32x QSFP+", 2, "4x QSFP+ 16x SFP+", 3, 3800, 3500, PlayerManager.ObjectInHand.Switch);

            // --- 4. SFP MODULE BOXES ---
            GUI.Label(new Rect(win.x + 20, win.y + 485, 500, 20), "<b>4. TRANSCEIVER BOXES (5 UNITS)</b>");
            DrawHardwareRow(win, 510, "SFP+ RJ45 Box", 0, "SFP+ Fiber Box", 1, 250, 350, PlayerManager.ObjectInHand.SFPBox);
            DrawHardwareRow(win, 555, "SFP28 Fiber Box", 2, "QSFP+ Fiber Box", 3, 900, 1500, PlayerManager.ObjectInHand.SFPBox);

            // --- 5. PATCH PANELS ---
            GUI.Label(new Rect(win.x + 20, win.y + 610, 500, 20), "<b>5. PATCH PANELS</b>");
            if (GUI.Button(new Rect(win.x + 20, win.y + 635, 245, 40), "RJ45 Panel ($250)")) PurchaseItem(0, 250, PlayerManager.ObjectInHand.PatchPanel, "Patch RJ45");
            if (GUI.Button(new Rect(win.x + 275, win.y + 635, 245, 40), "Combo Panel ($450)")) PurchaseItem(1, 450, PlayerManager.ObjectInHand.PatchPanel, "Patch Combo");
            if (GUI.Button(new Rect(win.x + 530, win.y + 635, 230, 40), "Fiber Panel ($450)")) PurchaseItem(2, 450, PlayerManager.ObjectInHand.PatchPanel, "Patch Fiber");

            // --- 6. INFRASTRUCTURE ---
            if (GUI.Button(new Rect(win.x + 20, win.y + 690, 740, 50), "<b>LANBERG RACK CABINET 19\" 47U ($1250)</b>"))
                PurchaseItem(0, 1250, PlayerManager.ObjectInHand.Rack, "Lanberg Rack 47U");

            // Footer / Branding
            GUI.Label(new Rect(win.x + 20, win.y + 750, 400, 20), "<size=10><color=grey>v0.9.0-BETA | Initial Public Release | by Lzinsp</color></size>");
            if (GUI.Button(new Rect(win.x + 540, win.y + 760, 220, 35), "<color=red><b>CLOSE TERMINAL (T)</b></color>"))
            {
                exibirTablet = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void DrawHardwareRow(Rect win, float y, string n1, int id1, string n2, int id2, int p1, int p2, PlayerManager.ObjectInHand type)
        {
            if (GUI.Button(new Rect(win.x + 20, win.y + y, 370, 35), $"{n1} - ${p1}")) PurchaseItem(id1, p1, type, n1);
            if (GUI.Button(new Rect(win.x + 400, win.y + y, 360, 35), $"{n2} - ${p2}")) PurchaseItem(id2, p2, type, n2);
        }

        private void PurchaseItem(int id, int price, PlayerManager.ObjectInHand type, string name, Color? color = null)
        {
            var shop = Object.FindObjectOfType<ComputerShop>();
            if (shop == null) return;
            Il2CppSystem.Nullable<Color> finalC = new Il2CppSystem.Nullable<Color>();
            if (color.HasValue) { finalC.value = color.Value; finalC.hasValue = true; }
            shop.SpawnNewCartItem(id, price, type, name, finalC);
            shop.UpdateCartTotal(); shop.ButtonCheckOut();
            MelonCoroutines.Start(TeleportToPlayer());
        }

        private IEnumerator TeleportToPlayer()
        {
            yield return new WaitForSeconds(0.9f);
            var pMan = Object.FindObjectOfType<PlayerManager>(); if (pMan == null) yield break;
            Vector3 pPos = pMan.transform.position;
            GameObject[] all = Resources.FindObjectsOfTypeAll<GameObject>();
            GameObject target = null;
            foreach (var obj in all)
            {
                if (obj.activeInHierarchy && obj.name.Contains("(Clone)"))
                {
                    // Identification filter
                    if (obj.name.Contains("Server") || obj.name.Contains("Cable") || obj.name.Contains("Rack") || obj.name.Contains("Switch") || obj.name.Contains("Patch") || obj.name.Contains("Box") || obj.name.Contains("SFP"))
                    {
                        if (Vector3.Distance(obj.transform.position, pPos) > 10f) { target = obj; break; }
                    }
                }
            }
            if (target != null)
            {
                Transform cam = Camera.main != null ? Camera.main.transform : pMan.transform;
                target.transform.position = cam.position + (cam.forward * 1.5f);
                target.transform.rotation = cam.rotation;
            }
        }
    }
}