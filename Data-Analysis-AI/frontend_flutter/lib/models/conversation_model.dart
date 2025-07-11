class ConversationModel {
  final String id;
  final String conversationName;
  final int timestamp;

  ConversationModel(this.id, this.conversationName, this.timestamp);

  ConversationModel.fromJson(Map<String, dynamic> json)
    : id = json['id'],
      conversationName = json['conversationName'],
      timestamp = json['timestamp'];
}
