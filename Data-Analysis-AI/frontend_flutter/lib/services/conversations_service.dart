import 'package:dio/dio.dart';
import 'package:frontend_flutter/models/conversation_model.dart';

class ConversationService {
  Future<List<ConversationModel>> getConversations() async {
    const String url = 'http://10.0.2.2:5045/api/AI/get-conversations';

    final dio = Dio();

    final response = await dio.get(url);

    if (response.statusCode != 200) {
      return Future.error('bir sorun olustu');
    }

    final List list = response.data;
    final List<ConversationModel> conversationList = list
        .map((e) => ConversationModel.fromJson(e))
        .toList();

    return conversationList;
  }

  Future<void> createConversation() async {
    const String url = 'http://10.0.2.2:5045/api/AI/create-conversation';

    final dio = Dio();

    final response = await dio.post(url);

    if (response.statusCode != 200) {
      return Future.error('bir sorun olustu');
    }
  }
}
