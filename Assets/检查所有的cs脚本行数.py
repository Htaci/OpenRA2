import os

def count_cs_lines(root_dir):
    total_lines = 0
    for dirpath, _, filenames in os.walk(root_dir):
        for filename in filenames:
            if filename.endswith('.cs'):
                file_path = os.path.join(dirpath, filename)
                try:
                    with open(file_path, 'r', encoding='utf-8') as f:
                        lines = f.readlines()
                        total_lines += len(lines)
                except Exception as e:
                    print(f"无法读取文件: {file_path}, 错误: {e}")
    return total_lines

if __name__ == "__main__":
    current_dir = os.path.dirname(os.path.abspath(__file__))
    lines = count_cs_lines(current_dir)
    print(f"所有.cs文件的总行数: {lines}")