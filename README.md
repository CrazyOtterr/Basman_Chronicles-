<code>
NewMapScene (корневая сцена)
│
├── LevelStateManager (корневой объект)
│   └── LevelStateManager.cs
│
├── Info Panel (полупрозрачная панель)
│
├── Canvas (основной холст)
│   ├── Canvas (содержит кнопки уровней)
│   │   ├── Уровень 1 (Button)
│   │   │   └── ButtonState.cs
│   │   │
│   │   └── Уровень 2 (Button)
│   │       └── ButtonState.cs
│   │
│   └── LevelMapManager.cs (на основном Canvas)


OurCabinet (сцена)
│
├── ===Managers===
│   ├── EventSystem
│   └── LevelManager //(мне он тут не нужен)
│
├── Canvas
│   └── Возврат (Button) ← вызывает LevelManager.ReturnToMap()


Mossovet(inside) (сцена)
│
├── Canvas
│   └── Возврат (Button) ← вызывает LevelManager.ReturnToMap()
├── EventSystem
├── LevelManager (пустой) ← LevelManager.cs
└── GameObject (пустой)
</code>

===Логика работы===
1. Запуск игры → LevelStateManager.ResetAllProgress() сбрасывает прогресс
- Первый уровень (OurCabinet) = Active (белый)
- Второй уровень (Mossovet(inside)) = Locked (красный)
2. Клик по кнопке уровня → ButtonState.OnPointerClick()
- Одинарный → LevelMapManager.ShowLevelInfo() → открывается Info Panel
- Двойной → ButtonState.LoadLevel() → загрузка уровня
3. Нажатие "Играть" на Info Panel → LevelMapManager.PlaySelectedLevel() → загрузка уровня
4. Нажатие "Возврат" на сцене уровня → LevelManager.ReturnToMap() → вызывает LevelStateManager.MarkLevelAsVisited() → уровень становится Completed (зеленый), следующий уровень становится Active (белый) → загрузка карты
5. Обновление цветов → OnLevelStateChanged событие → все ButtonState обновляют цвета
