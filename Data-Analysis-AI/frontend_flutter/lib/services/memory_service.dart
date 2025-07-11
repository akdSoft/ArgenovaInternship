import 'package:dio/dio.dart';
import 'package:frontend_flutter/models/memory_item_model.dart';

class MemoryService {
  Future<List<MemoryItemModel>> getMemoryItems() async {
    const String url = 'http://10.0.2.2:5045/api/AI/get-memoryitems';

    final dio = Dio();

    final response = await dio.get(url);

    if (response.statusCode != 200) {
      return Future.error('bir sorun olustu');
    }

    final List list = response.data;
    final List<MemoryItemModel> memoryItemList = list
        .map((e) => MemoryItemModel.fromJson(e))
        .toList();

    return memoryItemList;
  }
}
