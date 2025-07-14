import 'package:flutter/material.dart';
import 'package:frontend_flutter/models/memory_item_model.dart';
import 'package:frontend_flutter/services/memory_service.dart';
import 'package:frontend_flutter/widgets/custom_bar.dart';
import 'package:frontend_flutter/widgets/custom_drawer.dart';

class Memory extends StatefulWidget {
  const Memory({super.key});

  @override
  State<Memory> createState() => MemoryState();
}

class MemoryState extends State<Memory> {
  final GlobalKey<ScaffoldState> _scaffoldKey = GlobalKey<ScaffoldState>();

  bool _drawerOpened = false;

  late Future<List<MemoryItemModel>> memoryItems;

  Future<List<MemoryItemModel>> _loadMemoryItems() async {
    memoryItems = MemoryService().getMemoryItems();
    setState(() {});
    return memoryItems;
  }

  @override
  void initState() {
    super.initState();
    _loadMemoryItems();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      key: _scaffoldKey,
      drawer: CustomDrawer(),
      onDrawerChanged: (isOpened) {
        _drawerOpened = true;
        setState(() {});
        if (!isOpened) {
          _drawerOpened = false;
          setState(() {});
        }
      },
      backgroundColor: Color.fromARGB(255, 9, 6, 7),
      body: Stack(
        children: [
          CustomBar(
            scaffoldKey: _scaffoldKey,
            drawerOpened: _drawerOpened,
            title: 'Bellek',
          ),
          Positioned(
            top: 100,
            left: 0,
            right: 0,
            bottom: 20,
            child: SizedBox(
              height: 200,
              child: FutureBuilder<List<MemoryItemModel>>(
                future: _loadMemoryItems(),
                builder: (context, snapshot) {
                  if (snapshot.connectionState == ConnectionState.waiting) {
                    return Center(
                      child: CircularProgressIndicator(color: Colors.amber),
                    );
                  }

                  if (snapshot.hasError) {
                    return Center(
                      child: Text(
                        'Hata: ${snapshot.error}',
                        style: TextStyle(color: Colors.red),
                      ),
                    );
                  }

                  final memoryItems = snapshot.data!;

                  return Align(
                    alignment: Alignment.center,
                    child: ListView.builder(
                      padding: EdgeInsets.symmetric(
                        horizontal: 35,
                        vertical: 100,
                      ),
                      itemCount: memoryItems.length,
                      itemBuilder: (context, index) {
                        final conv = memoryItems[index];
                        return Container(
                          margin: EdgeInsets.only(bottom: 40),
                          padding: EdgeInsets.all(16),
                          decoration: BoxDecoration(
                            borderRadius: BorderRadius.circular(30),
                            border: Border.all(
                              color: const Color.fromARGB(255, 255, 255, 255),
                            ),
                          ),
                          child: Expanded(
                            child: Column(
                              children: [
                                Text(
                                  textAlign: TextAlign.center,
                                  conv.summary,
                                  style: TextStyle(
                                    color: Colors.white,
                                    fontSize: 20,
                                  ),
                                ),
                                SizedBox(height: 15),
                                Divider(indent: 20, endIndent: 20),
                                SizedBox(height: 15),
                                Text(
                                  textAlign: TextAlign.center,
                                  conv.date,
                                  style: TextStyle(
                                    color: Colors.white,
                                    fontSize: 20,
                                  ),
                                ),
                              ],
                            ),
                          ),
                        );
                      },
                    ),
                  );
                },
              ),
            ),
          ),
          _drawerOpened
              ? Positioned(
                  top: 0,
                  bottom: 0,
                  right: 0,
                  left: 0,
                  child: Container(color: Color.fromARGB(50, 255, 255, 255)),
                )
              : Positioned(top: 0, child: SizedBox.shrink()),
        ],
      ),
    );
  }
}
