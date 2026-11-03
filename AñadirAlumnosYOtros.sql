-- =========================
-- USUARIOS
-- =========================
INSERT INTO Usuarios (NombreUsuario, Email, Clave, Rol)
VALUES
('admin', 'admin@test.com', '123456', 'ADMIN'),
('operador', 'operador@test.com', '123456', 'OPERADOR');


-- =========================
-- ALUMNOS (50)
-- =========================
INSERT INTO Alumnos (Nombres, Apellidos, Ci, Celular, Email)
VALUES
('Juan','Perez','1000001','70000001','juan1@gmail.com'),
('Maria','Gomez','1000002','70000002','maria2@gmail.com'),
('Carlos','Lopez','1000003','70000003','carlos3@gmail.com'),
('Ana','Martinez','1000004','70000004','ana4@gmail.com'),
('Luis','Fernandez','1000005','70000005','luis5@gmail.com'),
('Sofia','Ramirez','1000006','70000006','sofia6@gmail.com'),
('Diego','Torrez','1000007','70000007','diego7@gmail.com'),
('Valeria','Rojas','1000008','70000008','valeria8@gmail.com'),
('Andres','Mendoza','1000009','70000009','andres9@gmail.com'),
('Paola','Vargas','1000010','70000010','paola10@gmail.com'),
('Miguel','Castro','1000011','70000011','miguel11@gmail.com'),
('Lucia','Ortega','1000012','70000012','lucia12@gmail.com'),
('Fernando','Soto','1000013','70000013','fernando13@gmail.com'),
('Gabriela','Flores','1000014','70000014','gabriela14@gmail.com'),
('Jorge','Quispe','1000015','70000015','jorge15@gmail.com'),
('Carla','Aguilar','1000016','70000016','carla16@gmail.com'),
('Ricardo','Navarro','1000017','70000017','ricardo17@gmail.com'),
('Daniela','Paredes','1000018','70000018','daniela18@gmail.com'),
('Pedro','Gutierrez','1000019','70000019','pedro19@gmail.com'),
('Elena','Salazar','1000020','70000020','elena20@gmail.com'),
('Oscar','Rivera','1000021','70000021','oscar21@gmail.com'),
('Patricia','Rios','1000022','70000022','patricia22@gmail.com'),
('Raul','Cruz','1000023','70000023','raul23@gmail.com'),
('Veronica','Herrera','1000024','70000024','veronica24@gmail.com'),
('Hector','Morales','1000025','70000025','hector25@gmail.com'),
('Natalia','Vega','1000026','70000026','natalia26@gmail.com'),
('Alberto','Campos','1000027','70000027','alberto27@gmail.com'),
('Silvia','Reyes','1000028','70000028','silvia28@gmail.com'),
('Mario','Delgado','1000029','70000029','mario29@gmail.com'),
('Claudia','Peña','1000030','70000030','claudia30@gmail.com'),
('Roberto','Arias','1000031','70000031','roberto31@gmail.com'),
('Adriana','Fuentes','1000032','70000032','adriana32@gmail.com'),
('Pablo','Mamani','1000033','70000033','pablo33@gmail.com'),
('Rosa','Choque','1000034','70000034','rosa34@gmail.com'),
('Eduardo','Condori','1000035','70000035','eduardo35@gmail.com'),
('Karla','Luna','1000036','70000036','karla36@gmail.com'),
('Javier','Cespedes','1000037','70000037','javier37@gmail.com'),
('Monica','Arce','1000038','70000038','monica38@gmail.com'),
('Victor','Tapia','1000039','70000039','victor39@gmail.com'),
('Lorena','Sanchez','1000040','70000040','lorena40@gmail.com'),
('Esteban','Ramos','1000041','70000041','esteban41@gmail.com'),
('Patricio','Lara','1000042','70000042','patricio42@gmail.com'),
('Andrea','Cordero','1000043','70000043','andrea43@gmail.com'),
('Bruno','Velasco','1000044','70000044','bruno44@gmail.com'),
('Camila','Escobar','1000045','70000045','camila45@gmail.com'),
('Felipe','Zamora','1000046','70000046','felipe46@gmail.com'),
('Daniel','Ibanez','1000047','70000047','daniel47@gmail.com'),
('Isabel','Mora','1000048','70000048','isabel48@gmail.com'),
('Tomas','Villarroel','1000049','70000049','tomas49@gmail.com'),
('Paula','Suarez','1000050','70000050','paula50@gmail.com');


-- =========================
-- CURSOS
-- =========================
INSERT INTO Cursos (Codigo, Nombre, FechaInicio, FechaFin)
VALUES
('CURS-001','Programación en C#','2026-01-10','2026-03-10'),
('CURS-002','Desarrollo Web Fullstack','2026-02-01','2026-04-01'),
('CURS-003','Base de Datos SQL Server','2026-03-01','2026-05-01');

