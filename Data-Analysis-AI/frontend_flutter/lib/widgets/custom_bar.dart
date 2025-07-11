import 'package:flutter/material.dart';

class CustomBar extends StatelessWidget {
  final GlobalKey<ScaffoldState> scaffoldKey;
  final bool drawerOpened;
  final String title;

  const CustomBar({
    super.key,
    required this.scaffoldKey,
    required this.drawerOpened,
    required this.title,
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
              child: Text(
                title,
                style: TextStyle(
                  color: Colors.white,
                  fontSize: 34,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
            SizedBox(
              height: 50,
              child: TextButton(onPressed: () {}, child: SizedBox.shrink()),
            ),
          ],
        ),
      ),
    );
  }
}
