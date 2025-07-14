import 'package:calendar_date_picker2/calendar_date_picker2.dart';
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

  final TextEditingController _textController = TextEditingController();
  final ScrollController _scrollController = ScrollController();

  Future<MessagepairModel?> _sendRequest() async {
    setState(() {
      isLoading = true;
    });

    final formattedDates = selectedDates
        .whereType<DateTime>()
        .map(
          (e) =>
              "${e.year}-${e.month.toString().padLeft(2, '0')}-${e.day.toString().padLeft(2, '0')}",
        )
        .toList();

    final prompt = _textController.text;

    if (prompt.trim().isEmpty) return null;

    final dio = Dio();
    final queryData = {
      "prompt": prompt,
      "conversationId": widget.conversationId,
      "selectedDays": formattedDates,
    };

    try {
      final response = await dio.post(
        "http://10.0.2.2:5045/api/AI/ask-ai-new",
        data: queryData,
      );

      setState(() {
        isLoading = false;
        _textController.text = '';
      });

      if (response.statusCode == 200) {
        setState(() {
          getMessagepairs();
        });
        return MessagepairModel.fromJson(response.data);
      }
    } catch (e) {
      showAboutDialog(context: context, applicationName: "Hata oluştu: $e");
      setState(() {
        isLoading = false;
        _textController.text = '';
      });
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
  bool isLoading = false;

  @override
  Widget build(BuildContext context) {
    final formattedDates = selectedDates
        .whereType<DateTime>()
        .map(
          (e) =>
              "${e.year}-${e.month.toString().padLeft(2, '0')}-${e.day.toString().padLeft(2, '0')}",
        )
        .toList();

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
                WidgetsBinding.instance.addPostFrameCallback((_) {
                  if (_scrollController.hasClients) {
                    _scrollController.jumpTo(
                      _scrollController.position.maxScrollExtent + 700,
                    );
                  }
                });

                return Align(
                  alignment: Alignment.center,
                  child: ListView.builder(
                    controller: _scrollController,
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
                                  child: Column(
                                    children: [
                                      Text(
                                        messagepairs[index].responseText,
                                        style: TextStyle(
                                          fontSize: 15,
                                          color: Colors.white,
                                        ),
                                      ),
                                      Container(
                                        margin: EdgeInsets.only(
                                          top: 30,
                                          bottom: 10,
                                        ),
                                        child: Divider(indent: 5, endIndent: 5),
                                      ),
                                      Row(
                                        children: [
                                          Container(
                                            margin: EdgeInsets.symmetric(
                                              horizontal: 10,
                                            ),
                                            child: Image.asset(
                                              'assets/icons/copy.png',
                                              width: 27,
                                              height: 27,
                                            ),
                                          ),
                                          Container(
                                            margin: EdgeInsets.symmetric(
                                              horizontal: 10,
                                            ),
                                            child: Image.asset(
                                              'assets/icons/refresh.png',
                                              width: 26,
                                              height: 26,
                                            ),
                                          ),
                                        ],
                                      ),
                                    ],
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
          CustomInput(
            controller: _textController,
            sendRequest: _sendRequest,
            openCalendar: _openCalendarDialog,
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
          isLoading
              ? Positioned(
                  bottom: 180,
                  right: 200,
                  child: CircularProgressIndicator(color: Colors.amber),
                )
              : Positioned(top: 0, child: SizedBox.shrink()),
        ],
      ),
    );
  }
}
