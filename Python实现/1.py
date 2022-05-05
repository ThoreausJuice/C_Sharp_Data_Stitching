#!/usr/bin/env python3

# 作为思路参考及可能的备用

import pandas as pd
from Data_Processing_Function import *

# 从A文件中找出特定字符串A_string_a对应的时间A_time_a
A_file = '测试文件\A.csv'
A_string_a = 's002'

A_first_processing = Basic_file_processing(A_file)

for x in A_first_processing:
    A_second_processing = x.split(',')
    if A_second_processing[0] == A_string_a:
        A_time_a = A_second_processing[1]

# 从B文件中找出时间A_time_a对应的数据B_data_a
B_file = '测试文件\B.csv'
B_data_a = []

B_first_processing = Basic_file_processing(B_file)

for x in B_first_processing:
    B_second_processing = x.split(',')
    if B_second_processing[0] == A_time_a:
        B_data_a.append(B_second_processing)

# 从C文件中找出时间A_time_a对应的数据C_data_a
C_file = '测试文件\C.csv'
C_data_a = []

C_first_processing = Basic_file_processing(C_file)

for x in C_first_processing:
    C_second_processing = x.split(',')
    if C_second_processing[0] == A_time_a:
        C_data_a.append(C_second_processing)

# 将B_data_a处理为 2变1 的数据
B_data_length = len(B_data_a)
B_data_a_processed = []
B_data_step = 2

for i in range(0, B_data_length, B_data_step):
    # 阶段A
    Stage_A = 0
    for j in range(B_data_step):
        Stage_A += float(B_data_a[i+j][1])
    
    B_data_a_processed.append(Stage_A / B_data_step)

# print(B_data_a_processed)

# 将C_data_a处理为 1变1 的数据
C_data_length = len(C_data_a)
C_data_a_processed = []
C_data_step = 1

for i in range(0, C_data_length, C_data_step):
    # 阶段B
    Stage_B = 0
    for j in range(C_data_step):
        Stage_B += float(C_data_a[i+j][1])
    
    C_data_a_processed.append(Stage_B / C_data_step)

# print(C_data_a_processed)

# 将时间、A阶段、B阶段 数据进行拼接

D_data_a_processed = []
D_data_length = len(C_data_a)

column_name_to_be_written = []
column_name_to_be_written.append('时间')
column_name_to_be_written.append('阶段A')
column_name_to_be_written.append('阶段B')

for i in range(D_data_length):
    D_data_a = []
    D_data_a.append(A_time_a)
    D_data_a.append(B_data_a_processed[i])
    D_data_a.append(C_data_a_processed[i])
    D_data_a_processed.append(D_data_a)

# print(D_data_a_processed)

D_data_a_to_be_written = pd.DataFrame(columns=column_name_to_be_written, data=D_data_a_processed)
D_data_a_to_be_written.to_csv('已处理完成/D.csv', encoding='utf-8', index=False)