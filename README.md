### Перед запуском
Перед запуском поменять пароль в файле [docker-compose.yml](https://github.com/mipesync/Koshelek.Ru.TestTask/blob/master/docker-compose.yml) (через поиск все **dbPassword** заменить на необходимые)
### Ссылки на сервисы
1. Бэк со Swagger документацией (**backend-api**): http://localhost:5241/swagger/index.html
2. Клиент, отображающий сообщения в реальном времени (**client-observer**): http://localhost:5044/
3. Клиент, выводящий историю сообщений (**client-historian**. Можно выбрать временной отрезок): http://localhost:5190/
   
Клиент, пишуший потоком сообщения (**client-writer**), реализовал в виде фонового воркера, интерфейса нет. Работоспособность отслеживать через консоль
