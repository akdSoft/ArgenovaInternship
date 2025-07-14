import 'package:flutter/material.dart';
import 'package:frontend_flutter/models/messagepair_model.dart';

class CustomInput extends StatelessWidget {
  final TextEditingController controller;
  final Future<MessagepairModel?> Function() sendRequest;
  final void Function() openCalendar;

  const CustomInput({
    super.key,
    required this.controller,
    required this.sendRequest,
    required this.openCalendar,
  });

  @override
  Widget build(BuildContext context) {
    return Positioned(
      bottom: 20,
      left: 0,
      right: 0,
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceEvenly,
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          SizedBox(
            width: 300,
            child: TextField(
              keyboardType: TextInputType.multiline,
              maxLines: null,
              controller: controller,
              style: TextStyle(color: Colors.white),
              decoration: InputDecoration(
                hintText: 'Soru sor...',
                hintStyle: TextStyle(
                  color: const Color.fromARGB(255, 74, 74, 74),
                  fontSize: 18,
                ),
                contentPadding: EdgeInsets.all(20),
                enabledBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(30),
                  borderSide: BorderSide(
                    color: const Color.fromARGB(255, 255, 255, 255),
                    width: 1,
                  ),
                ),
                focusedBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(30),
                  borderSide: BorderSide(
                    color: const Color.fromARGB(255, 255, 255, 255),
                    width: 1,
                  ),
                ),
              ),
            ),
          ),
          Column(
            children: [
              SizedBox(
                height: 53,
                child: TextButton(
                  onPressed: () async {
                    await sendRequest(); // isteği tetikle
                  },
                  child: Image.asset('assets/icons/send.png'),
                ),
              ),
              SizedBox(
                height: 60,
                child: TextButton(
                  onPressed: openCalendar,
                  child: Image.asset('assets/icons/calendar.png'),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}
