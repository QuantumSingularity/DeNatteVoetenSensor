SELECT last_value FROM public."Sensors_SensorId_seq";
SELECT last_value FROM public."SensorLogItems_SensorLogItemId_seq";
SELECT last_value FROM public."Users_UserId_seq";
SELECT last_value FROM public."UserLogItems_UserLogItemId_seq";


select max("SensorId") from public."Sensors";
select max("SensorLogItemId") from public."SensorLogItems";
select max("UserId") from public."Users";
select max("UserLogItemId") from public."UserLogItems";



ALTER SEQUENCE public."SensorLogItems_SensorLogItemId_seq" MINVALUE 35000 START WITH 35000 RESTART WITH 35000;

    