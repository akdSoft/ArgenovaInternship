import 'package:dio/dio.dart';
import 'package:file_picker/file_picker.dart';
import 'package:flutter/material.dart';
import 'package:frontend_flutter/widgets/custom_bar.dart';
import 'package:frontend_flutter/widgets/custom_drawer.dart';

class Addnewday extends StatefulWidget {
  const Addnewday({super.key});

  @override
  State<Addnewday> createState() => AddnewdayState();
}

class AddnewdayState extends State<Addnewday> {
  final GlobalKey<ScaffoldState> _scaffoldKey = GlobalKey<ScaffoldState>();

  bool _drawerOpened = false;
  bool isLoading = false;
  bool uploaded = false;

  Future<void> uploadExcel() async {
    setState(() => isLoading = true);

    final result = await FilePicker.platform.pickFiles(
      type: FileType.custom,
      allowedExtensions: ['xlsx', 'xls'],
    );

    if (result == null) {
      // print("Dosya seçilmedi.");
      showAboutDialog(context: context);
      return;
    }

    final filePath = result.files.single.path!;
    final fileName = result.files.single.name;

    final dio = Dio();

    try {
      final formData = FormData.fromMap({
        "excelFile": await MultipartFile.fromFile(filePath, filename: fileName),
      });

      final response = await dio.post(
        "http://10.0.2.2:5045/api/AI/add-new-day",
        data: formData,
        options: Options(contentType: "multipart/form-data"),
      );

      if (response.statusCode == 200) {
        // print("Dosya başarıyla yüklendi.");
        // showAboutDialog(context: context, applicationName: 'basarili');
      } else {
        // print("Yükleme başarısız: ${response.statusCode}");
        // showAboutDialog(context: context, applicationName: 'basarisiz');
      }
    } catch (e) {
      // print("Hata oluştu: $e");
      // showAboutDialog(context: context, applicationName: '${e}');
    } finally {
      setState(() {
        isLoading = false;
        uploaded = true;
      });
    }
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
          _drawerOpened
              ? Positioned(
                  top: 0,
                  bottom: 0,
                  right: 0,
                  left: 0,
                  child: Container(color: Color.fromARGB(255, 43, 43, 43)),
                )
              : Positioned(top: 0, child: SizedBox.shrink()),
          CustomBar(
            scaffoldKey: _scaffoldKey,
            drawerOpened: _drawerOpened,
            title: 'Yeni Gün Ekle',
          ),
          Positioned(
            top: 380,
            left: 40,
            right: 40,
            bottom: 330,
            child: GestureDetector(
              onTap: () async {
                await uploadExcel();
              },
              child: Container(
                width: 500,
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(45),
                  border: Border.all(color: Colors.white),
                ),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Image.asset('assets/icons/upload.png', scale: 7),
                    Text(
                      textAlign: TextAlign.center,
                      'Yeni bir günün\nraporunu yükleyin...\n(.xls .xlsx .csv)',
                      style: TextStyle(
                        color: const Color.fromARGB(255, 167, 167, 167),
                        fontSize: 18,
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ),
          if (isLoading)
            Positioned(
              bottom: 200,
              left: 0,
              right: 0,
              child: Center(
                child: CircularProgressIndicator(color: Colors.amber),
              ),
            ),
          if (uploaded)
            Positioned(
              bottom: 100,
              left: 0,
              right: 0,
              child: Center(
                child: Text(
                  'Dosyanız yüklendi...',
                  style: TextStyle(
                    color: const Color.fromARGB(255, 129, 254, 133),
                  ),
                ),
              ),
            ),
        ],
      ),
    );
  }
}
