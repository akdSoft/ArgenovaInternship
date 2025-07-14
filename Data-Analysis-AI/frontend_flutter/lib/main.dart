import 'package:flutter/material.dart';
import 'package:frontend_flutter/screens/AddNewDay.dart';
import 'package:frontend_flutter/screens/conversations.dart';
import 'package:frontend_flutter/screens/memory.dart';
import 'package:frontend_flutter/widgets/calendar.dart';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      theme: ThemeData(fontFamily: 'Inter'),
      initialRoute: '/conversations',
      routes: {
        '/addnewday': (context) => const Addnewday(),
        '/conversations': (context) => const Conversations(),
        '/memory': (context) => const Memory(),
        '/test': (context) => const DatePickerCalendar(),
        // '/chat': (context) => const Chat(),
      },
    );
  }
}
