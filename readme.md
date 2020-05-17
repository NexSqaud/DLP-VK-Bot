# DLP VK Bot [![C#](https://img.shields.io/badge/language-C%23-brightgreen.svg)](https://en.wikipedia.org/wiki/C_Sharp_(programming_language)) [![Windows](https://img.shields.io/badge/platform-Windows-0078d7.svg)](https://en.wikipedia.org/wiki/Microsoft_Windows) [![License](https://img.shields.io/github/license/NexSqaud/OsirisUpdate.svg)](LICENSE)

DLP ВК бот для пользователей. Написан на C# с использованием библеоткети [VkNet](https://github.com/vknet/vk) и [VkNetExtend](https://github.com/CaCTuCaTu4ECKuu/VkNetExtend)

ВК разработчика [https://vk.com/ronamchornog](https://vk.com/ronamchornog)

Команда BlowOut Studio: [https://vk.com/blowoutstudio](https://vk.com/blowoutstudio)

## Запуск

1. В файле Program.cs замените "ТОКЕН" и `userId = 0` на свой токен и ID ВК. Необходимы разрешения к сообщениям и друзьям.
2. Скомпилируйте проект в Release режиме
3. Запустите vkBot.exe из папки Release

## Команды
{обязательный параметр}, {ENTER} - перенос строки, [несколько поддерживаемых параметров]
| Команда | Описание |
|:--------:|:--------:|
| .помощь | Показывает все команды. |
| .пинг | Показывает задержку от ВК до бота. (бывает врет) |
| .лс | Пишет сообщение себе в ЛС. |
| дд {число} | Удаляет последние {число} сообщений. |
| .шаб | Показывает список шаблонов. |
| .шаб {название} | Заменяет сообщение шаблоном {название} |
| +шаб {название} {ENTER} {шаблон} | Добавляет шаблон {название}. Поддерживаются вложения. |
| -шаб {название} | Удаляет шаблон {название}. |
| .дшаб | Показывает список динамических шаблонов. |
| .дшаб {название} | Запускает динамический шаблон {название}. |
| +дшаб {название} {ENTER} {кадры} | Добавляет динамический шаблон {название}. Кадры отделяются пустой строкой. |
| -дшаб {название} | Удаляет динамический шаблон {название}. |
| .игнор | Показывает список игнорируемых пользователей. |
| +игнор [упоминание или ответ] | Добавляет пользователя в список игнорируемых. |
| -игнор [упоминание или ответ] | Удаляет пользователя из списка игнорируемых. |
| +др [упоминание или ответ] | Добавляет пользователя в друзья/отправляет заявку в друзья. |
| -др [упоминание или ответ] | Удаляет пользователя из друзей/отменяет заявку в друзья. |
| +чс [упоминание или ответ] | Добавляет пользователя в черный список. |
| -чс [упоминание или ответ] | Удаляет пользователя из черного списка. |
