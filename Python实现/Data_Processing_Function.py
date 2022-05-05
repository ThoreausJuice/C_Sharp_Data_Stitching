#!/usr/bin/env python3

# 用于记录处理文件常用的函数

# 函数功能：对文件进行基本处理，形成以回车为分隔符的数组
def Basic_file_processing(file_name):
    # 加载训练集
    with open(file_name, 'r', encoding = 'utf-8') as b:
        b_original_string = b.read()

    b_first_processing = b_original_string.split('\n')
    
    # 处理掉文件结尾的空值
    if b_first_processing[-1] == '':
        b_first_processing.pop()
    
    return b_first_processing