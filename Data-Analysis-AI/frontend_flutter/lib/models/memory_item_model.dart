class MemoryItemModel {
  final int id;
  final String summary;
  final String date;

  MemoryItemModel(this.id, this.summary, this.date);

  MemoryItemModel.fromJson(Map<String, dynamic> json)
    : id = json['id'],
      summary = json['summary'],
      date = json['date'];
}
