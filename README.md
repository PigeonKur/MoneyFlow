# Финансовый трекер

Приложение для управления личными финансами, разработанное на *Blazor Server* с использованием Entity Framework Core и Chart.js для визуализации данных.

---

## Описание проекта

> "Контролируй свои доходы и расходы — управляй будущим!"

Финансовый трекер помогает пользователю:

* анализировать свои доходы и расходы по категориям;
* отслеживать инвестиции и переводы;
* строить графики динамики баланса за различные периоды.

---

## Основные возможности

1. Просмотр транзакций за выбранный период
2. Диаграммы доходов и расходов
3. История изменения баланса
4. Интеграция с API
5. Раздел для инвестиций

---

## Структура проекта

| Папка/файл     | Назначение                          |
| -------------- | ----------------------------------- |
| Controllers/ | API-контроллеры (BalanceController) |
| Pages/       | Razor-компоненты интерфейса         |
| Models/      | Классы и DTO                        |
| wwwroot/js/  | Скрипты Chart.js                    |
| README.md    | Документация проекта                |

---

## Компоненты

* MainScreen.razor — главный экран с диаграммами
* TransactionDto.cs — модель транзакции
* BalanceController.cs — логика расчета баланса

---

## Установка и запуск

1. Клонируй репозиторий:

  
   git clone https://github.com/PigeonKur/MoneyFlow.git
   
2. Перейди в каталог проекта:

  
   cd MoneyFlow
   
3. Настрой подключение к базе данных в *appsettings.json*

4. Запусти миграции и приложение:

  
   dotnet ef database update
   dotnet run
   
---

## Используемые технологии

* ASP.NET Core
* Blazor Server
* Entity Framework Core
* Chart.js
* SQL Server

---

## Пример кода
```
[HttpGet("dashboard-data")]
public async Task<IActionResult> GetDashboardData(int userId, string period = "month")
{
    var startDate = GetStartDate(period, DateTime.UtcNow);
    var transactions = await _DbContext.Transactions
        .Where(t => t.UserId == userId && t.TransactionDate >= startDate)
        .ToListAsync();

    return Ok(transactions);
}
```
---
## Визуализация

![Пример Главного экрана](<img width="1369" height="663" alt="image" src="https://github.com/user-attachments/assets/2423a0f3-a872-47ef-aab6-957cf4693dd9" />)

![Пример Личного кабинета](<img width="1075" height="413" alt="image" src="https://github.com/user-attachments/assets/897a48cc-91a7-4c50-9970-7cb4f2a692db" />)

![Пример Истории](<img width="1313" height="646" alt="image" src="https://github.com/user-attachments/assets/41f45a54-5baf-4c9a-bdcd-b8810a74b363" />)

![Пример Инвестиций](<img width="1288" height="626" alt="image" src="https://github.com/user-attachments/assets/640b4aa5-b6dc-455d-a952-82a168ab21f9" />)

---

## Планы по улучшению

* [x] Основная логика контроллера
* [x] Диаграммы доходов и расходов
* [x] Авторизация пользователей
* [ ] Настройки валют
* [ ] Импорт CSV-файлов

---

## Полезные ссылки

* [Документация Blazor](https://learn.microsoft.com/aspnet/core/blazor)
* [Chart.js](https://www.chartjs.org/)
* [Entity Framework Core](https://learn.microsoft.com/ef/core)

---

## Пример списков

### Нумерованный

1. Создай проект
2. Добавь модель
3. Подключи контроллер
4. Настрой API

### Ненумерованный

* Транзакции
* Баланс
* Инвестиции
* Отчеты

### Вложенный

* Клиентская часть

  * Razor-компоненты
  * Chart.js
* Серверная часть

  * Контроллеры
  * Контекст БД

---
