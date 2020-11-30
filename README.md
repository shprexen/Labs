# Labs

Была реализована система конфигурации сервиса в формате XML и JSON.
В XML и JSON файлах находятся конфигурационные свойства SourcePath(directory), TargetPath(directory), Arhcive(принимает true/false), Encrypt(принимает true/false).
Реализован интерфейс IConfigurationParser, который наследуют парсеры в моей программе.
Созданы: класс ConfigurationOptions, который хранит конфигурационные свойства, класс ConfigurationProvider, определяющий с каким парсером надо работать проге.
