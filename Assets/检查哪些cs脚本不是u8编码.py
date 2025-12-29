import os

def is_utf8(file_path):
    """
    检查文件是否为UTF-8编码
    """
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            f.read()
        return True
    except UnicodeDecodeError:
        return False

def process_directory(directory):
    """
    遍历目录及其子目录中的所有.cs文件
    """
    for root, dirs, files in os.walk(directory):
        for file in files:
            if file.endswith('.cs'):
                file_path = os.path.join(root, file)
                if not is_utf8(file_path):
                    print(f"文件 {file_path} 不是UTF-8编码.")

if __name__ == "__main__":
    # 获取脚本所在的目录
    script_directory = os.path.dirname(os.path.abspath(__file__))
    print(f"正在处理目录：{script_directory}")
    process_directory(script_directory)