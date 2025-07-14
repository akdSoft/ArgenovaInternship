import 'package:flutter/material.dart';
import 'package:calendar_date_picker2/calendar_date_picker2.dart';

class DatePickerCalendar extends StatefulWidget {
  const DatePickerCalendar({super.key});

  @override
  State<DatePickerCalendar> createState() => DatePickerCalendarState();
}

class DatePickerCalendarState extends State<DatePickerCalendar> {
  List<DateTime?> selectedDates = [];

  void _openCalendarDialog() async {
    final results = await showDialog<List<DateTime?>>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Tarihleri Seç'),
        content: SizedBox(
          width: 300,
          child: CalendarDatePicker2(
            config: CalendarDatePicker2Config(
              calendarType: CalendarDatePicker2Type.multi,
              selectedDayHighlightColor: Colors.blueAccent,
              firstDayOfWeek: 1,
              weekdayLabels: ['Paz', 'Pzt', 'Sal', 'Çar', 'Per', 'Cum', 'Cmt'],
            ),
            value: selectedDates,
            onValueChanged: (dates) {
              setState(() => selectedDates = dates);
            },
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, selectedDates),
            child: const Text('TAMAM'),
          ),
        ],
      ),
    );

    if (results != null) {
      setState(() {
        selectedDates = results;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    final formattedDates = selectedDates
        .whereType<DateTime>()
        .map(
          (e) =>
              "${e.year}-${e.month.toString().padLeft(2, '0')}-${e.day.toString().padLeft(2, '0')}",
        )
        .toList();

    return Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        ElevatedButton(
          onPressed: _openCalendarDialog,
          child: const Text('Tarih Seç'),
        ),
        const SizedBox(height: 20),
        if (formattedDates.isNotEmpty) ...formattedDates.map((d) => Text(d)),
      ],
    );
  }
}
