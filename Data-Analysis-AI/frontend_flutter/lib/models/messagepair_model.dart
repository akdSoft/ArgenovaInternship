class MessagepairModel {
  final String id;
  final String conversationId;
  final String prompt;
  final String responseText;
  final List<String> selectedDays;
  final int duration;
  final int date;

  MessagepairModel(
    this.id,
    this.conversationId,
    this.prompt,
    this.responseText,
    this.selectedDays,
    this.duration,
    this.date,
  );

  MessagepairModel.fromJson(Map<String, dynamic> json)
    : id = json['id'],
      conversationId = json['conversationId'],
      prompt = json['prompt'],
      responseText = json['responseText'],
      selectedDays = List<String>.from(json['selectedDays'] ?? []),
      duration = json['duration'],
      date = json['date'];
}
