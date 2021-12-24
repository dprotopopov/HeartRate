# Определение частоты сердечных сокращений методом корреляции сигнала измерений сердечных сокращений с использованием быстрых Фурье преобразований 

## Введение

При обработке медицинских данных требуется определять частоту сердечных сокращений (ЧСС). Большинство методик расчёта ЧСС использует определение пиков в графике сердечных сокращений и подсчёта длительности интервала между пиками. Альтернативным методом расчёта ЧСС является вычисление корреляции последовательности измерений относительно сдвига графика на заданный интервал времении и выбор в качестве вычисленного интервала того, при котором корреляция максимальная. Недостатком вычисления интервала сердечных сокращений методом рассчёта корреляции является большое число вычислений, однако число этих расчётов можно существенно сократить при использовании быстрых Фурье преобразований (БФП).

## Описание метода определения частоты сердечных сокращений

Путь x(i) - последовательность измерений сердечных сокращений полученные с частотой дискретизации frequency (Гц).

### Предварительная обработка сигнала

Обработаем последовательность x фильтром двойного дифференцирования, то есть вычислим f(t)=x(t)-2\*x(t-1)+x(t-2).
Как указано в работе (Котов Максим Дмитриевич. Алгоритм подсчета ЧСС на основе данных ЭКГ, полученных бесконтактно. Бакалаврская работа. 2015) при данной предварительной фильтрации наиболее качественно детектируется длительнось интервала сердечных сокращений.

### Вычисление длительности интервала сердечных сокращений

Для вычисления ЧСС на заданный момент t требуется выбрать длительность окна данных, на котором производится определение интервала частоты сердечных сокращений. Пусть длительность этого интервала определена переменной delay (сек) и эта величина выбирается порядка ~3 сек исходя из диапазона ЧСС в 40-200 ударов/мин. Тогда нам понадобится выделить из исходной последовательности N = ЦЕЛОЕ(delay\*frequency) измерений на момент t.

- Подготовим массив длины 2N F(i) = { f(t-(N-1)+i) при 0<=i<N и 0 при N<=i<2N }

- Вычислим массив S(k) = SUM F(i)\*F((i+k) mod 2N) для  i=0,2N-1

Для вычисления массива S применяем алгоритм с использованием БФП

1. Вычисляем массив V = FFT(F), где FFT - прямое Фурье-преобразование
2. Вычисляем массив M2(i) = {|V(i)|^2}
3. Вычисляем массив S = BFT(M2), где BFT - обратное Фурье-преобразование

- Вычислим массив корреляций P(k) = S(k) /(1+|N-k|)

- В массиве P(k), для индексов в диапазоне \[60/200\*frequency;MIN(N,60/40\*frequency)\] найдём индекс index с максимальным значением P(index).

Тогда длительность интервала сердечных сокращений в момент t равна index/frequency (сек), а частота сердечных сокращений, соответственно, равна 60\*frequency/index (ударов/мин).

#### Примечания

- Коэффициент 1/(1+|N-k|) при вычислении P(k) выбран поскольку в сумме SUM F(i)\*F((i+k) mod 2N) участвует не более |N-k| ненулевых слагаемых.

- При вычислении Фурье-преобразований можно не заботится об умножении на поправочный коэффициент, поскольку алгоритм предполагает выбор максимального значения из массива P.

## Теоретические основы метода

### Корреляция двух функций

Согласно определению, корреляцией (F,G) двух функций F и G называется величина: 

- (F,G) = SUM F(i)\*G(i)

### Свёртка двух функций

Согласно определению, свёрткой двух функций F и G называется функция FхG:

- FхG(t) = SUM F(i)\*G(j)|i+j=t

## Дискретное преобразование Фурье

Дискретное преобразование Фурье (в англоязычной литературе DFT, Discrete Fourier Transform) — это одно из преобразований Фурье, широко применяемых в алгоритмах цифровой обработки сигналов (его модификации применяются в сжатии звука в MP3, сжатии изображений в JPEG и др.), а также в других областях, связанных с анализом частот в дискретном (к примеру, оцифрованном аналоговом) сигнале. Дискретное преобразование Фурье требует в качестве входа дискретную функцию. Такие функции часто создаются путём дискретизации (выборки значений из непрерывных функций). Дискретные преобразования Фурье помогают решать дифференциальные уравнения в частных производных и выполнять такие операции, как свёртки. Дискретные преобразования Фурье также активно используются в статистике, при анализе временных рядов. Существуют многомерные дискретные преобразования Фурье.

### Фурье-преобразования для вычисления свёртки

Одним из замечательных свойств преобразований Фурье является возможность быстрого вычисления корреляции двух функций определённых, либо на действительном аргументе (при использовании классической формулы), либо на конечном кольце (при использовании дискретных преобразований).
И хотя подобные свойства присущи многим линейным преобразованиям, для практического применения, для вычисления операции свёртки, согласно данному нами определению, используется формула

- FхG = BFT ( FFT(F)\*FFT(G) )

Где

- FFT – операция прямого преобразования Фурье
- BFT – операция обратного преобразования Фурье

Проверить правильность равенства довольно легко – явно подставив в формулы Фурье-преобразований и сократив получившиеся формулы 

### Фурье-преобразования для вычисления корреляции

Пусть (F,G)(t) равна корреляции получаемой в результате сдвига одного вектора, относительно другого на шаг t
Тогда, как уже показано ранее, выполняется 

- (F,G)(t) = FхG’(t) = BFT ( FFT(F)\*FFT(G’) )

Если используются реализации алгоритма трансформации Фурье через комплексные числа, то такие преобразования обладают ещё одним замечательным свойством:

- FFT(G’) = CONJUGATE ( FFT(G) )

Где 

- CONJUGATE ( FFT(G) ) – матрица, составленная из сопряжённых элементов матрицы FFT(G)

Таким образом, получаем

- (F,G)(t) = BFT ( FFT(F)\*CONJUGATE ( FFT(G) ))

### Фурье-преобразования для вычисления корреляции функции с собой

Из ранее полученного имеем

- (F,F)(t) = FхF’(t) = BFT ( FFT(F)\*FFT(F’) )
- FFT(F') = CONJUGATE ( FFT(F) )

Таким образом, если 

- V = FFT( F ) и 
- M2 = V\*CONJUGATE(V) = {|V(i)|^2}, 

то 

- (F,F)(t) = BFF ( V\*CONJUGATE(V) ) = BFT ( M2 )

## Пример реализации

Реализован пример обработки файла в формате EDF, содержащего канал EKG с данными измерений сердечных сокращений https://github.com/dprotopopov/HeartRate.
Частота сердечных сокращений рассчитывается консольной программой с заданной частотой на основе измерений, содержащихся в EDF файле.
Результыты сравнивались с результатами в других программах обработки медицинских данных.

## Используемая литература

- Дмитрий Протопопов. Использование БФП для цифровой обработки изображений. https://github.com/dprotopopov/FFTTools
- Котов Максим Дмитриевич. Алгоритм подсчета ЧСС на основе данных ЭКГ, полученных бесконтактно. Бакалаврская работа. 2015 https://se.math.spbu.ru/thesis/texts/_Kotov_Maksim_Dmitrievich_Bachelor_Thesis_2015_text.pdf

## Используемое программное обеспечение

- Microsoft Visual Studio 2019 C# - среда и язык программирования
- Math.Net – библиотека реализующая алгоритмы быстрого дискретного преобразования Фурье
