# AdvertisingWebService

Простой веб-сервис для хранения и поиска рекламных площадок по заданной локации.  
Данные загружаются из файла и хранятся в оперативной памяти (in-memory).

---

## Структура проекта

- `AdvertisingWebService` — основной проект с API.
- `AdvertisingWebService.Tests` — юнит-тесты для API контроллера.

---

## Требования

- .NET 9 SDK
- Любой HTTP-клиент для тестирования API (Postman, curl, браузер)

---

## Сборка и запуск проекта
Для запуска необходимо убедиться в том что установлен .net 9 или выше
```
dotnet --version
```
Клоинруем репозиторий
```
git clone https://github.com/serejka164/TestTask-AdvertisingWebService
cd TestTask-AdvertisingWebService
```
Компиляция
```
dotnet build
```
Запуск тестов
```
dotnet test AdvertisingWebService.Tests
```
Запуск проекта
```
dotnet run --project AdvertisingWebService
```
После запуска видим строчку говорящую на каком порту работает приложение
Now listening on: http://localhost:5240
Проект поддерживает Swagger поэтому можно перейти по адресму http://localhost:5240/swagger
и тестировать работу проекта

## Использование API
Проект содержит в себе три энпоинта
### Загрузка файла от пользователя (Upload)
```
POST /api/v1/advertising/upload
Content-Type: multipart/form-data
File: <выбранный файл>
```
Ответ: 200 OK при успешной загрузке, 400 BadRequest или 500 Internal Server Error при ошибках.
### Загрузка рекламных файла из папки с проектом (Load)
в репозитории уже лежит файл ads.txt 
```
POST /api/v1/advertising/load?pathToFile=<путь к файлу>
```
Параметр pathToFile — путь на диске до файла с площадками.
Ответ: 200 OK при успешной загрузке, 400 BadRequest или 500 Internal Server Error при ошибках.

### Поиск рекламных площадок по локации (Search)
```
GET /api/v1/advertising/search?location=<локация>
```
Пример: /api/v1/advertising/search?location=/ru/svrd/revda
Возвращает список площадок, действующих в указанной локации.
Ответ: 200 OK со списком площадок.


## Пример файла для загрузки

```
Яндекс.Директ:/ru
Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik
Газета уральских москвичей:/ru/msk,/ru/permobl,/ru/chelobl
Крутая реклама:/ru/svrd
```
