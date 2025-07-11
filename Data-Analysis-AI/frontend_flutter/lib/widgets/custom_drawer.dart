import 'package:flutter/material.dart';
import 'package:frontend_flutter/screens/AddNewDay.dart';
import 'package:frontend_flutter/screens/conversations.dart';
import 'package:frontend_flutter/screens/memory.dart';

class CustomDrawer extends StatelessWidget {
  const CustomDrawer({super.key});

  @override
  Widget build(BuildContext context) {
    var route = ModalRoute.of(context);
    String? routeName;

    if (route != null) {
      routeName = route.settings.name;
    }

    return Drawer(
      backgroundColor: Colors.black,
      child: Column(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            padding: EdgeInsets.all(20),
            child: Column(
              spacing: 40,
              children: [
                SizedBox(
                  width: 200,
                  child: ShaderMask(
                    shaderCallback: (bounds) => const LinearGradient(
                      begin: Alignment.topLeft,
                      end: Alignment.bottomRight,
                      colors: [
                        Color.fromARGB(255, 192, 114, 113), // Açık mavi
                        Color.fromARGB(255, 171, 30, 212), // Mor
                      ],
                    ).createShader(bounds),
                    child: const Text(
                      'Girin-AI',
                      style: TextStyle(
                        fontSize: 35,
                        fontWeight: FontWeight.w800,
                        color: Colors.white,
                      ),
                    ),
                  ),
                ),
                SizedBox(width: 220, child: Divider(indent: 0)),
                SizedBox(
                  width: 200,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    spacing: 20,
                    children: [
                      GestureDetector(
                        onTap: () {
                          Navigator.pushReplacement(
                            context,
                            MaterialPageRoute(
                              builder: (context) => Memory(),
                              settings: RouteSettings(name: '/memory'),
                            ),
                          );
                        },
                        child: Text(
                          'Bellek',
                          style: routeName == '/memory'
                              ? TextStyle(
                                  color: const Color.fromARGB(
                                    255,
                                    128,
                                    128,
                                    128,
                                  ),
                                  fontSize: 25,
                                )
                              : TextStyle(color: Colors.white, fontSize: 25),
                        ),
                      ),
                      GestureDetector(
                        onTap: () {
                          Navigator.pushReplacement(
                            context,
                            MaterialPageRoute(
                              builder: (context) => Conversations(),
                              settings: RouteSettings(name: '/conversations'),
                            ),
                          );
                        },
                        child: Text(
                          'Sohbetler',
                          style: routeName == '/conversations'
                              ? TextStyle(
                                  color: const Color.fromARGB(
                                    255,
                                    128,
                                    128,
                                    128,
                                  ),
                                  fontSize: 25,
                                )
                              : TextStyle(color: Colors.white, fontSize: 25),
                        ),
                      ),
                      GestureDetector(
                        onTap: () {
                          Navigator.pushReplacement(
                            context,
                            MaterialPageRoute(
                              builder: (context) => Addnewday(),
                              settings: RouteSettings(name: '/addnewday'),
                            ),
                          );
                        },
                        child: Text(
                          'Yeni Gün Ekle',
                          style: routeName == '/addnewday'
                              ? TextStyle(
                                  color: const Color.fromARGB(
                                    255,
                                    128,
                                    128,
                                    128,
                                  ),
                                  fontSize: 25,
                                )
                              : TextStyle(color: Colors.white, fontSize: 25),
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),

          Column(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              SizedBox(width: 240, child: Divider(indent: 20)),
              Container(
                padding: EdgeInsets.all(20),
                child: Row(
                  spacing: 20,
                  children: [
                    CircleAvatar(radius: 20),
                    Text(
                      'Admin',
                      style: TextStyle(color: Colors.white, fontSize: 25),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}
