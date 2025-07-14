import 'package:flutter/material.dart';
import 'package:frontend_flutter/models/conversation_model.dart';
import 'package:frontend_flutter/screens/chat.dart';
import 'package:frontend_flutter/services/conversations_service.dart';
import 'package:frontend_flutter/widgets/custom_bar.dart';
import 'package:frontend_flutter/widgets/custom_drawer.dart';

class Conversations extends StatefulWidget {
  const Conversations({super.key});

  @override
  State<Conversations> createState() => ConversationsState();
}

class ConversationsState extends State<Conversations> {
  final GlobalKey<ScaffoldState> _scaffoldKey = GlobalKey<ScaffoldState>();

  bool _drawerOpened = false;

  late Future<List<ConversationModel>> conversations;

  Future<List<ConversationModel>> _loadConversations() async {
    conversations = ConversationService().getConversations();
    setState(() {});
    return conversations;
  }

  Future<void> _createConversation() async {
    ConversationService().createConversation();
    setState(() {
      conversations = ConversationService().getConversations();
    });
  }

  @override
  void initState() {
    super.initState();
    conversations = ConversationService().getConversations();
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
            title: 'Sohbetler',
          ),
          Positioned(
            top: 100,
            left: 0,
            right: 0,
            bottom: 150,
            child: SizedBox(
              height: 200,
              child: FutureBuilder<List<ConversationModel>>(
                future: conversations,
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

                  final conversations = snapshot.data!;

                  return Align(
                    alignment: Alignment.center,
                    child: ListView.builder(
                      padding: EdgeInsets.symmetric(
                        horizontal: 35,
                        vertical: 100,
                      ),
                      itemCount: conversations.length,
                      itemBuilder: (context, index) {
                        final conv = conversations[index];
                        return GestureDetector(
                          onTap: () => {
                            Navigator.pushReplacement(
                              context,
                              MaterialPageRoute(
                                builder: (context) => Chat(
                                  conversationId: conv.id,
                                  conversationName: conv.conversationName,
                                ),
                                settings: RouteSettings(name: '/chat'),
                              ),
                            ),
                          },
                          child: Container(
                            height: 70,
                            margin: EdgeInsets.only(bottom: 40),
                            padding: EdgeInsets.all(16),
                            decoration: BoxDecoration(
                              borderRadius: BorderRadius.circular(30),
                              border: Border.all(
                                color: const Color.fromARGB(255, 255, 255, 255),
                              ),
                            ),
                            child: Row(
                              mainAxisAlignment: MainAxisAlignment.spaceBetween,
                              children: [
                                SizedBox(
                                  width: 220,
                                  child: Text(
                                    conv.conversationName,
                                    style: TextStyle(
                                      color: Colors.white,
                                      fontSize: 20,
                                    ),
                                    overflow: TextOverflow.ellipsis,
                                    maxLines: 1,
                                  ),
                                ),
                                TextButton(
                                  style: TextButton.styleFrom(
                                    padding: EdgeInsets.zero,
                                  ),
                                  onPressed: () {},
                                  child: Image.asset(
                                    'assets/icons/trash.png',
                                    width: 30,
                                    height: 30,
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
          Positioned(
            bottom: 35,
            left: 0,
            right: 0,
            child: Stack(
              children: [
                Center(
                  child: SizedBox(
                    width: 90,
                    child: Image.asset('assets/icons/plus.png'),
                  ),
                ),
                GestureDetector(
                  onTap: () {
                    _createConversation();
                  },
                  child: Center(
                    child: Container(
                      width: 90,
                      height: 90,
                      decoration: BoxDecoration(
                        borderRadius: BorderRadius.circular(60),
                        color: Colors.amberAccent.withAlpha(0),
                      ),
                    ),
                  ),
                ),
              ],
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
