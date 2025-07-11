import 'dart:ui';

import 'package:flutter/material.dart';

class CustomChatBar extends StatelessWidget {
  final GlobalKey<ScaffoldState> scaffoldKey;
  final bool drawerOpened;
  final String convoTitle;

  const CustomChatBar({
    super.key,
    required this.scaffoldKey,
    required this.drawerOpened,
    required this.convoTitle,
  });

  @override
  Widget build(BuildContext context) {
    return Positioned(
      top: 0,
      left: 0,
      right: 0,
      child: Container(
        padding: EdgeInsets.only(top: 20),
        height: 90,
        color: drawerOpened
            // ? Color.fromARGB(255, 43, 43, 43)
            ? const Color.fromARGB(255, 0, 0, 0)
            : const Color.fromARGB(255, 0, 0, 0),
        alignment: Alignment.center,
        child: Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            SizedBox(
              height: 40,
              child: TextButton(
                onPressed: () {
                  scaffoldKey.currentState?.openDrawer();
                },
                child: Image.asset('assets/icons/menu.png'),
              ),
            ),
            Container(
              padding: EdgeInsets.only(right: 30),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.center,
                crossAxisAlignment: CrossAxisAlignment.center,
                spacing: 10,
                children: [
                  CircleAvatar(
                    backgroundImage: Image.asset('assets/icons/ai.png').image,
                  ),
                  Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        convoTitle,
                        style: TextStyle(color: Colors.white, fontSize: 24),
                      ),
                      Text(
                        'GirinAI',
                        style: TextStyle(color: Colors.white, fontSize: 15),
                      ),
                    ],
                  ),
                ],
              ),
            ),
            SizedBox(
              height: 50,
              child: TextButton(
                onPressed: () {},
                child: Image.asset('assets/icons/three_dot.png'),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
