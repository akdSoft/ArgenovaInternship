// import 'package:dio/dio.dart';
// import 'package:frontend_flutter/models/messagepair_model.dart';

// class ChatService {
//   Future<List<MessagepairModel>> getMessagepairs(String convoId) async {
//     final String url = 'http://10.0.2.2:5045/api/AI/get-messagepairs/$convoId';

//     try {
//       final response = await Dio().get(url);

//       // response.data: List<Map<String, dynamic>>
//       final List<dynamic> data = response.data;

//       return data.map((json) => MessagepairModel.fromJson(json)).toList();
//     } catch (e) {
//       print("Hata: $e");
//       return [];
//     }
//   }

//   Future<MessagepairModel?> askAI(
//     String prompt,
//     String convoId,
//     List<String> selectedDays,
//   ) async {
//     final dio = Dio();

//     final queryData = {
//       "prompt": prompt,
//       "conversationId": convoId,
//       "selectedDays": selectedDays,
//     };

//     try {
//       final response = await dio.post(
//         "http://10.0.2.2:5045/api/AI/ask-ai-new",
//         data: queryData,
//       );

//       if (response.statusCode == 200) {
//         // print("Dosya başarıyla yüklendi.");
//         // showAboutDialog(context: context, applicationName: 'basarili');
//         return MessagepairModel.fromJson(response.data);
//       } else {
//         // print("Yükleme başarısız: ${response.statusCode}");
//         // showAboutDialog(context: context, applicationName: 'basarisiz');
//       }
//     } catch (e) {
//       // print("Hata oluştu: $e");
//       // showAboutDialog(context: context, applicationName: '${e}');
//     }
//   }
// }
