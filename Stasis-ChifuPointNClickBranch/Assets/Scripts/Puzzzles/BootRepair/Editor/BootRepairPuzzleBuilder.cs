#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public static class BootRepairPuzzleBuilder
{
    private const string CANVAS_NAME  = "For UI canvas";
    private const string SP_BOOT      = "Assets/Sprites/BootPuzzle/boot.png";
    private const string SP_SLOT      = "Assets/Sprites/BootPuzzle/slot_cell.png";
    private const string SP_ITEM      = "Assets/Sprites/BootPuzzle/item_placeholder.png";
    private const string SP_DRAWER_BG = "Assets/Sprites/BootPuzzle/drawer_bg.png";

    // ─── Menu items ───────────────────────────────────────────────────────────

    [MenuItem("Tools/BootRepair/Build Puzzle UI")]
    public static void BuildPuzzleUI()
    {
        Canvas canvas = FindCanvas();
        if (canvas == null) return;

        GameObject existing = GameObject.Find("BootRepairPuzzle");
        if (existing != null)
        {
            bool replace = EditorUtility.DisplayDialog(
                "BootRepairPuzzle уже существует",
                "Удалить и пересоздать?", "Да", "Отмена");
            if (!replace) return;
            Undo.DestroyObjectImmediate(existing);
        }

        // Принудительно переимпортируем спрайты как Sprite перед загрузкой
        ForceImportSprites();

        Sprite spBoot     = LoadSprite(SP_BOOT);
        Sprite spSlot     = LoadSprite(SP_SLOT);
        Sprite spItem     = LoadSprite(SP_ITEM);
        Sprite spDrawerBg = LoadSprite(SP_DRAWER_BG);

        GameObject root       = BuildRoot(canvas.transform);
        GameObject background = BuildBackground(root.transform);
        GameObject panel      = BuildPanel(root.transform);

        BuildTitle(panel.transform);

        GameObject drawer         = BuildDrawer(panel.transform, spDrawerBg);
        GameObject rightZone      = BuildRightZone(panel.transform);
        BuildBootImage(rightZone.transform, spBoot);
        GameObject slotsContainer = BuildSlotsContainer(rightZone.transform);
        GameObject checkBtn       = BuildCheckButton(panel.transform);
        GameObject resultTxt      = BuildResultText(panel.transform);
        GameObject closeBtn       = BuildCloseButton(panel.transform);

        BootRepairItem[] items = BuildDrawerItems(drawer.transform, spItem);
        (BootSlot[] slots, _) = BuildSlots(slotsContainer.transform, spSlot);

        WireController(root, checkBtn, resultTxt, items, slots);
        WirePuzzle(root, panel, background);
        WireCloseButton(closeBtn, root);

        root.SetActive(false);
        Undo.RegisterCreatedObjectUndo(root, "Build BootRepair Puzzle UI");
        Selection.activeGameObject = root;

        Debug.Log("[BootRepairPuzzleBuilder] Готово. Tools > BootRepair > Open Puzzle для предпросмотра.");
    }

    [MenuItem("Tools/BootRepair/Open Puzzle (Preview)")]
    public static void OpenPuzzlePreview()
    {
        GameObject root = GameObject.Find("BootRepairPuzzle");
        if (root == null) { EditorUtility.DisplayDialog("Не найдено", "Сначала Build Puzzle UI.", "OK"); return; }
        root.SetActive(true);
        Selection.activeGameObject = root;
    }

    [MenuItem("Tools/BootRepair/Close Puzzle (Preview)")]
    public static void ClosePuzzlePreview()
    {
        GameObject root = GameObject.Find("BootRepairPuzzle");
        if (root != null) root.SetActive(false);
    }

    // ─── Корень ──────────────────────────────────────────────────────────────

    static GameObject BuildRoot(Transform parent)
    {
        GameObject go = new GameObject("BootRepairPuzzle");
        go.transform.SetParent(parent, false);
        Stretch(go.AddComponent<RectTransform>());
        go.AddComponent<Puzzle>();
        go.AddComponent<BootRepairPuzzleController>();
        return go;
    }

    static GameObject BuildBackground(Transform parent)
    {
        GameObject go = new GameObject("Background");
        go.transform.SetParent(parent, false);
        Stretch(go.AddComponent<RectTransform>());
        go.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
        return go;
    }

    // ─── Панель ──────────────────────────────────────────────────────────────

    static GameObject BuildPanel(Transform parent)
    {
        GameObject go = new GameObject("PuzzlePanel");
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot     = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(800f, 540f);
        rt.anchoredPosition = Vector2.zero;
        go.AddComponent<Image>().color = new Color(0.11f, 0.08f, 0.06f, 0.97f);
        return go;
    }

    static void BuildTitle(Transform parent)
    {
        GameObject go = new GameObject("Title");
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot     = new Vector2(0.5f, 1f);
        rt.sizeDelta = new Vector2(0f, 44f);
        rt.anchoredPosition = new Vector2(0f, -4f);
        TMP_Text tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = "Починка сапога";
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize  = 22f;
        tmp.color     = new Color(0.95f, 0.85f, 0.65f);
        tmp.fontStyle = FontStyles.Bold;
    }

    // ─── Ящик (левая сторона) ────────────────────────────────────────────────

    static GameObject BuildDrawer(Transform parent, Sprite bg)
    {
        GameObject go = new GameObject("Drawer");
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(0.42f, 1f);
        rt.offsetMin = new Vector2(14f, 56f);   // снизу отступ под кнопку
        rt.offsetMax = new Vector2(-6f, -52f);  // сверху под заголовок
        Image img = go.AddComponent<Image>();
        img.sprite = bg;
        img.type   = Image.Type.Simple;
        img.color  = new Color(0.18f, 0.13f, 0.09f, 1f);
        GridLayoutGroup grid = go.AddComponent<GridLayoutGroup>();
        grid.cellSize        = new Vector2(90f, 90f);
        grid.spacing         = new Vector2(8f, 8f);
        grid.padding         = new RectOffset(14, 14, 14, 14);
        grid.constraint      = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 3;
        return go;
    }

    // ─── Правая зона ─────────────────────────────────────────────────────────

    static GameObject BuildRightZone(Transform parent)
    {
        GameObject go = new GameObject("RightZone");
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.43f, 0f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.offsetMin = new Vector2(6f, 56f);
        rt.offsetMax = new Vector2(-14f, -52f);
        return go;
    }

    static void BuildBootImage(Transform parent, Sprite bootSprite)
    {
        GameObject go = new GameObject("BootImage");
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 0.38f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.offsetMin = new Vector2(8f, 0f);
        rt.offsetMax = new Vector2(-8f, -8f);
        Image img = go.AddComponent<Image>();
        img.sprite         = bootSprite;
        img.preserveAspect = true;
        img.color          = Color.white;
        // Если спрайт не загрузился — тёмный заменитель, чтобы не висело белое
        if (bootSprite == null) img.color = new Color(0.2f, 0.15f, 0.1f);
    }

    static GameObject BuildSlotsContainer(Transform parent)
    {
        GameObject go = new GameObject("BootSlots");
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(1f, 0.36f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        GridLayoutGroup grid = go.AddComponent<GridLayoutGroup>();
        grid.cellSize        = new Vector2(98f, 98f);
        grid.spacing         = new Vector2(10f, 10f);
        grid.padding         = new RectOffset(8, 8, 8, 8);
        grid.childAlignment  = TextAnchor.MiddleCenter;
        grid.constraint      = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 4;
        return go;
    }

    // ─── Кнопка "Проверить" ──────────────────────────────────────────────────

    static GameObject BuildCheckButton(Transform parent)
    {
        GameObject go = new GameObject("CheckButton");
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0.42f, 0f);
        rt.anchorMax        = new Vector2(1f, 0f);
        rt.pivot            = new Vector2(0.5f, 0f);
        rt.sizeDelta        = new Vector2(0f, 44f);
        rt.anchoredPosition = new Vector2(0f, 8f);
        Image img = go.AddComponent<Image>();
        img.color = new Color(0.18f, 0.50f, 0.18f, 1f);
        go.AddComponent<Button>();
        AddLabel(go.transform, "Проверить", 18f);
        return go;
    }

    // ─── Текст результата ────────────────────────────────────────────────────

    static GameObject BuildResultText(Transform parent)
    {
        GameObject go = new GameObject("ResultText");
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0f, 0f);
        rt.anchorMax        = new Vector2(0.41f, 0f);
        rt.pivot            = new Vector2(0f, 0f);
        rt.sizeDelta        = new Vector2(0f, 44f);
        rt.anchoredPosition = new Vector2(14f, 8f);
        TMP_Text tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = "";
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize  = 17f;
        tmp.color     = Color.white;
        go.SetActive(false);
        return go;
    }

    // ─── Кнопка закрытия ────────────────────────────────────────────────────

    static GameObject BuildCloseButton(Transform parent)
    {
        GameObject go = new GameObject("CloseButton");
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin        = new Vector2(1f, 1f);
        rt.anchorMax        = new Vector2(1f, 1f);
        rt.pivot            = new Vector2(1f, 1f);
        rt.sizeDelta        = new Vector2(38f, 38f);
        rt.anchoredPosition = new Vector2(-8f, -8f);
        go.AddComponent<Image>().color = new Color(0.65f, 0.12f, 0.12f, 1f);
        Button btn = go.AddComponent<Button>();
        // Transition None — кнопка не меняет цвет сама
        btn.transition = Selectable.Transition.None;
        AddLabel(go.transform, "X", 20f);
        return go;
    }

    // ─── Предметы в ящике ────────────────────────────────────────────────────

    static BootRepairItem[] BuildDrawerItems(Transform parent, Sprite placeholder)
    {
        BootRepairItem[] items = new BootRepairItem[9];
        string[] names = { "Нос", "Подошва", "Задник", "Стелька", "Гвоздь", "Смола", "Ткань", "Пряжка", "Войлок" };
        Color[] tints =
        {
            new Color(0.95f, 0.75f, 0.40f),
            new Color(0.40f, 0.65f, 0.95f),
            new Color(0.70f, 0.45f, 0.95f),
            new Color(0.40f, 0.85f, 0.55f),
            new Color(0.55f, 0.55f, 0.55f),
            new Color(0.45f, 0.35f, 0.28f),
            new Color(0.38f, 0.42f, 0.55f),
            new Color(0.65f, 0.58f, 0.25f),
            new Color(0.48f, 0.32f, 0.32f),
        };

        for (int i = 0; i < 9; i++)
        {
            GameObject go = new GameObject($"Item_{i}");
            go.transform.SetParent(parent, false);

            Image img = go.AddComponent<Image>();
            img.sprite         = placeholder;
            img.color          = tints[i];
            img.type           = Image.Type.Simple;
            img.preserveAspect = false;
            // Если спрайта нет — просто цветной квадрат, уже норм
            if (placeholder == null) img.sprite = null;

            // Transition None — Unity не будет сам перекрашивать кнопку в белый
            Button btn = go.AddComponent<Button>();
            btn.transition = Selectable.Transition.None;

            BootRepairItem item = go.AddComponent<BootRepairItem>();
            SerializedObject so = new SerializedObject(item);
            so.FindProperty("itemId").intValue = i;
            so.ApplyModifiedPropertiesWithoutUndo();

            // Название снизу
            GameObject lbl    = new GameObject("Name");
            lbl.transform.SetParent(go.transform, false);
            RectTransform lrt = lbl.AddComponent<RectTransform>();
            lrt.anchorMin = new Vector2(0f, 0f);
            lrt.anchorMax = new Vector2(1f, 0.32f);
            lrt.offsetMin = Vector2.zero;
            lrt.offsetMax = Vector2.zero;
            TMP_Text tmp  = lbl.AddComponent<TextMeshProUGUI>();
            tmp.text      = names[i];
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize  = 11f;
            tmp.color     = Color.white;

            items[i] = item;
        }
        return items;
    }

    // ─── Слоты сапога ────────────────────────────────────────────────────────

    static (BootSlot[], GameObject[]) BuildSlots(Transform parent, Sprite slotSprite)
    {
        BootSlot[]   slots    = new BootSlot[4];
        GameObject[] overlays = new GameObject[4];
        string[]     names    = { "Нос", "Подошва", "Задник", "Стелька" };
        Color[]      tints    =
        {
            new Color(0.95f, 0.75f, 0.40f, 0.40f),
            new Color(0.40f, 0.65f, 0.95f, 0.40f),
            new Color(0.70f, 0.45f, 0.95f, 0.40f),
            new Color(0.40f, 0.85f, 0.55f, 0.40f),
        };

        for (int i = 0; i < 4; i++)
        {
            GameObject go = new GameObject($"Slot_{i}");
            go.transform.SetParent(parent, false);

            Image img = go.AddComponent<Image>();
            img.sprite = slotSprite;
            img.color  = tints[i];
            img.type   = Image.Type.Simple;

            // Transition None — не мешать кастомной логике фидбека
            Button btn = go.AddComponent<Button>();
            btn.transition = Selectable.Transition.None;

            BootSlot slot = go.AddComponent<BootSlot>();
            SerializedObject so = new SerializedObject(slot);
            so.FindProperty("slotIndex").intValue = i;
            so.ApplyModifiedPropertiesWithoutUndo();

            // Overlay — ПЕРВЫЙ дочерний (BootSlot.Awake: GetChild(0))
            GameObject overlay = new GameObject("Overlay");
            overlay.transform.SetParent(go.transform, false);
            Stretch(overlay.AddComponent<RectTransform>());
            Image oimg = overlay.AddComponent<Image>();
            oimg.color   = Color.clear;
            oimg.enabled = false;
            overlay.transform.SetSiblingIndex(0);

            // Подпись
            GameObject lbl    = new GameObject("SlotLabel");
            lbl.transform.SetParent(go.transform, false);
            RectTransform lrt = lbl.AddComponent<RectTransform>();
            lrt.anchorMin = new Vector2(0f, 0f);
            lrt.anchorMax = new Vector2(1f, 0.3f);
            lrt.offsetMin = Vector2.zero;
            lrt.offsetMax = Vector2.zero;
            TMP_Text tmp  = lbl.AddComponent<TextMeshProUGUI>();
            tmp.text      = names[i];
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize  = 11f;
            tmp.color     = new Color(1f, 1f, 1f, 0.85f);

            slots[i]    = slot;
            overlays[i] = overlay;
        }
        return (slots, overlays);
    }

    // ─── Привязка компонентов ────────────────────────────────────────────────

    static void WireController(GameObject root, GameObject checkBtn, GameObject resultTxt,
                                BootRepairItem[] items, BootSlot[] slots)
    {
        BootRepairPuzzleController ctrl = root.GetComponent<BootRepairPuzzleController>();
        SerializedObject so = new SerializedObject(ctrl);
        so.FindProperty("checkButton").objectReferenceValue = checkBtn.GetComponent<Button>();
        so.FindProperty("resultText").objectReferenceValue  = resultTxt.GetComponent<TMP_Text>();
        SerializedProperty dp = so.FindProperty("drawerItems");
        dp.arraySize = items.Length;
        for (int i = 0; i < items.Length; i++) dp.GetArrayElementAtIndex(i).objectReferenceValue = items[i];
        SerializedProperty sp = so.FindProperty("bootSlots");
        sp.arraySize = slots.Length;
        for (int i = 0; i < slots.Length; i++) sp.GetArrayElementAtIndex(i).objectReferenceValue = slots[i];
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static void WirePuzzle(GameObject root, GameObject panel, GameObject background)
    {
        Puzzle puzzle = root.GetComponent<Puzzle>();
        SerializedObject so = new SerializedObject(puzzle);
        so.FindProperty("_puzzlePanel").objectReferenceValue      = panel;
        so.FindProperty("_puzzleBackground").objectReferenceValue = background.GetComponent<Image>();
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static void WireCloseButton(GameObject closeBtn, GameObject root)
    {
        Button btn    = closeBtn.GetComponent<Button>();
        Puzzle puzzle = root.GetComponent<Puzzle>();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, puzzle.Disable);
    }

    // ─── Утилиты ─────────────────────────────────────────────────────────────

    static void ForceImportSprites()
    {
        string[] paths = { SP_BOOT, SP_SLOT, SP_ITEM, SP_DRAWER_BG };
        foreach (string path in paths)
        {
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            if (ti == null)
            {
                Debug.LogWarning($"[BootRepairPuzzleBuilder] Файл не найден: {path}");
                continue;
            }
            ti.textureType         = TextureImporterType.Sprite;
            ti.spriteImportMode    = SpriteImportMode.Single;
            ti.alphaIsTransparency = true;
            ti.mipmapEnabled       = false;
            // ForceSynchronousImport — блокирует поток до завершения импорта,
            // иначе LoadAssetAtPath<Sprite> вернёт null
            AssetDatabase.ImportAsset(path,
                ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
        }
        AssetDatabase.Refresh();
    }

    static Sprite LoadSprite(string path)
    {
        // Сначала пробуем прямую загрузку
        Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (s != null) return s;

        // Для Single-mode sprite субассет лежит внутри текстуры — ищем через LoadAllAssetsAtPath
        foreach (Object obj in AssetDatabase.LoadAllAssetsAtPath(path))
        {
            if (obj is Sprite sprite) return sprite;
        }

        Debug.LogWarning($"[BootRepairPuzzleBuilder] Спрайт не загружен: {path}. " +
                         "Выдели файл в Project > ПКМ > Reimport, потом пересобери.");
        return null;
    }

    static Canvas FindCanvas()
    {
        GameObject go = GameObject.Find(CANVAS_NAME);
        if (go != null) { Canvas c = go.GetComponent<Canvas>(); if (c != null) return c; }
        Canvas[] all = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        if (all.Length > 0)
        {
            Debug.LogWarning($"[BootRepairPuzzleBuilder] Canvas '{CANVAS_NAME}' не найден, использую '{all[0].name}'.");
            return all[0];
        }
        EditorUtility.DisplayDialog("Canvas не найден", $"Нет Canvas '{CANVAS_NAME}'. Открой сцену.", "OK");
        return null;
    }

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    static void AddLabel(Transform parent, string text, float size)
    {
        GameObject lbl   = new GameObject("Label");
        lbl.transform.SetParent(parent, false);
        RectTransform rt = lbl.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        TMP_Text tmp  = lbl.AddComponent<TextMeshProUGUI>();
        tmp.text      = text;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize  = size;
        tmp.color     = Color.white;
    }
}
#endif
