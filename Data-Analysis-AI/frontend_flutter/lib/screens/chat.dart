import 'dart:convert';

import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:frontend_flutter/models/messagepair_model.dart';
import 'package:frontend_flutter/widgets/custom_drawer.dart';
import 'package:frontend_flutter/widgets/input.dart';
import 'package:frontend_flutter/widgets/custom_chat_bar.dart';

class Chat extends StatefulWidget {
  final String conversationId;
  final String conversationName;
  const Chat({
    super.key,
    required this.conversationId,
    required this.conversationName,
  });

  @override
  State<Chat> createState() => ChatState();
}

class ChatState extends State<Chat> {
  final TextEditingController _controller = TextEditingController();

  Future<MessagepairModel?> _sendRequest() async {
    final prompt = _controller.text;

    if (prompt.trim().isEmpty) return null;

    final dio = Dio();
    final queryData = {
      "prompt": prompt,
      "conversationId": widget.conversationId,
      "selectedDays": ["2025-07-01"],
    };

    try {
      final response = await dio.post(
        "http://10.0.2.2:5045/api/AI/ask-ai-new",
        data: queryData,
      );

      if (response.statusCode == 200) {
        return MessagepairModel.fromJson(response.data);
      }
    } catch (e) {
      showAboutDialog(context: context, applicationName: "Hata oluştu: $e");
    }

    return null;
  }

  List<MessagepairModel> messagepairs = [];

  Future<List<MessagepairModel>> getMessagepairs() async {
    final String url =
        'http://10.0.2.2:5045/api/AI/get-messagepairs/${widget.conversationId}';

    try {
      final response = await Dio().get(url);

      final List list = response.data;
      final List<MessagepairModel> messagepairList = list
          .map((e) => MessagepairModel.fromJson(e))
          .toList();

      return messagepairList;
    } catch (e) {
      showAboutDialog(context: context, applicationName: "Hata oluştu: $e");
      return [];
    }
  }

  @override
  void initState() {
    super.initState();
    getMessagepairs();
  }

  final GlobalKey<ScaffoldState> _scaffoldKey = GlobalKey<ScaffoldState>();

  bool _drawerOpened = false;

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
          CustomChatBar(
            scaffoldKey: _scaffoldKey,
            drawerOpened: _drawerOpened,
            convoTitle: widget.conversationName,
          ),
          Positioned(
            top: 100,
            left: 0,
            right: 0,
            bottom: 130,
            child: FutureBuilder<List<MessagepairModel>>(
              future: getMessagepairs(),
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

                final messagepairs = snapshot.data!;
                return Align(
                  alignment: Alignment.center,
                  child: ListView.builder(
                    padding: EdgeInsets.symmetric(vertical: 100),
                    itemCount: messagepairs.length,
                    itemBuilder: (context, index) {
                      return Container(
                        margin: EdgeInsets.only(bottom: 35, left: 5, right: 5),
                        child: Column(
                          spacing: 35,
                          children: [
                            Row(
                              mainAxisAlignment: MainAxisAlignment.end,
                              crossAxisAlignment: CrossAxisAlignment.end,
                              spacing: 10,
                              children: [
                                Container(
                                  width: 320,
                                  padding: EdgeInsets.all(16),
                                  decoration: BoxDecoration(
                                    gradient: LinearGradient(
                                      begin: Alignment.topLeft,
                                      end: Alignment.bottomRight,
                                      colors: [
                                        Color(0xFFC07271),
                                        Color(0xFFAB1ED4),
                                      ],
                                    ),
                                    borderRadius: BorderRadius.circular(28),
                                  ),
                                  child: Text(
                                    messagepairs[index].prompt,
                                    style: TextStyle(
                                      fontSize: 15,
                                      color: Colors.white,
                                    ),
                                  ),
                                ),
                                CircleAvatar(
                                  radius: 12,
                                  backgroundColor: Colors.grey.shade200,
                                  child: ClipOval(
                                    child: SizedBox(
                                      width: 20,
                                      height: 20,
                                      child: Image.asset(
                                        'assets/icons/user.png',
                                        fit: BoxFit.cover,
                                      ),
                                    ),
                                  ),
                                ),
                              ],
                            ),
                            Row(
                              mainAxisAlignment: MainAxisAlignment.start,
                              crossAxisAlignment: CrossAxisAlignment.end,
                              spacing: 10,
                              children: [
                                CircleAvatar(
                                  radius: 12,
                                  backgroundImage: Image.asset(
                                    'assets/icons/ai.png',
                                  ).image,
                                ),
                                Container(
                                  width: 320,
                                  padding: EdgeInsets.all(16),
                                  decoration: BoxDecoration(
                                    borderRadius: BorderRadius.circular(28),
                                    color: Color(0xFF222021),
                                  ),
                                  child: Text(
                                    messagepairs[index].responseText,
                                    style: TextStyle(
                                      fontSize: 15,
                                      color: Colors.white,
                                    ),
                                  ),
                                ),
                              ],
                            ),
                          ],
                        ),
                      );
                    },
                  ),
                );
              },
            ),
          ),
          CustomInput(controller: _controller, sendRequest: _sendRequest),
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
