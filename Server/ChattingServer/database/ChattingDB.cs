using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattingServer.database
{
    public class ChattingDB
    {
		private string _connectionAddress;

		public ChattingDB()
        {
            string _server = "localhost"; //DB 서버 주소
            int _port = 3306; //DB 서버 포트
            string _database = "chatting"; //DB 이름
            string _id = "root"; //계정 아이디
            string _pw = "root"; //계정 비밀번호

            _connectionAddress = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", _server, _port, _database, _id, _pw);
        }

		/// <summary>
		/// 회원가입 가능한지 확인
		/// </summary>
		/// <param name="id">대상 아이디</param>
		public bool IsJoinPossible(string id)
		{
			try
			{
				DataSet dataSet = new DataSet();

				using (MySqlConnection mySql = new MySqlConnection(_connectionAddress))
				{
					mySql.Open();

					//같은 아이디가 DB에 존재하지 않으면 회원가입 가능
					string query = "SELECT * FROM user_tb WHERE id = @id ;";

					MySqlCommand cmd = new MySqlCommand(query, mySql);

					MySqlParameter param = new MySqlParameter("@id", MySqlDbType.VarChar);
					param.Value = id;
					cmd.Parameters.Add(param);

					MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
					adapter.Fill(dataSet);

					if (dataSet.Tables[0].Rows.Count > 0)
					{
						return false;
					}
					else
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);

				return false;
			}
		}

		/// <summary>
		/// 회원 정보 저장
		/// </summary>
		/// <param name="id">아이디</param>
		/// <param name="pw">비밀번호</param>
		/// <param name="name">이름</param>
		public void InsertUser(int clientUid, string id, string pw, string name)
        {
			try
			{
				using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
				{
					mysql.Open();
					
					string query 
						= string.Format("INSERT INTO user_tb (uid, id, pw, name) VALUES ('{0}', '{1}', '{2}', '{3}');", clientUid, id, pw, name);

					MySqlCommand cmd = new MySqlCommand(query, mysql);

					if (cmd.ExecuteNonQuery() != 1)
					{
						Debug.WriteLine("회원 정보 저장 실패");
					}
                    else
                    {
						Debug.WriteLine("회원 정보 저장 완료");
                    }
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		///// <summary>
		///// 모든 회원 조회
		///// </summary>
		//public DataSet GetAllUsers()
		//{
		//	DataSet dataSet = new DataSet();

		//	try
		//	{
		//		using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
		//		{
		//			string sql = "SELECT * FROM user_tb";
		//			MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sql, mysql);
		//			dataAdapter.Fill(dataSet, "user_tb");
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		MessageBox.Show(ex.Message);
		//	}

		//	return dataSet;
		//}


		/// <summary>
		/// 로그인 가능한지 확인
		/// </summary>
		/// <param name="id">아이디</param>
		/// <param name="pw">비밀번호</param>
		public bool IsLoginPossible(string id, string pw)
		{
			try
			{
				DataSet dataSet = new DataSet();

				using (MySqlConnection mySql = new MySqlConnection(_connectionAddress))
				{
					mySql.Open();

					//아이디, 비밀번호가 DB의 값과 일치하면 로그인
					string query = "SELECT * FROM user_tb WHERE id = @id AND pw = @pw;";

					MySqlCommand cmd = new MySqlCommand(query, mySql);

					MySqlParameter idParam = new MySqlParameter("@id", MySqlDbType.VarChar);
					idParam.Value = id;
					cmd.Parameters.Add(idParam);

					MySqlParameter pwParam = new MySqlParameter("@pw", MySqlDbType.VarChar);
					pwParam.Value = pw;
					cmd.Parameters.Add(pwParam);

					MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
					adapter.Fill(dataSet);

					if (dataSet.Tables[0].Rows.Count > 0)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);

				return false;
			}
		}

		/// <summary>
		/// 로그인 상태 정보 업데이트
		/// </summary>
		/// <param name="id">대상 아이디</param>
		public void UpdateUserLoginState(string id, bool isLogin)
		{
			int loginState = isLogin ? 1 : 0;

			try
			{
				using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
				{
					mysql.Open();

					string query = "UPDATE connection_tb SET login_state = @loginState WHERE user_id = @id";

					MySqlCommand cmd = new MySqlCommand(query, mysql);

					MySqlParameter loginStateParam = new MySqlParameter("@loginState", MySqlDbType.Int32);
					loginStateParam.Value = loginState;
					cmd.Parameters.Add(loginStateParam);

					MySqlParameter idParam = new MySqlParameter("@id", MySqlDbType.VarChar);
					idParam.Value = id;
					cmd.Parameters.Add(idParam);

					if (cmd.ExecuteNonQuery() != 1)
					{
						Debug.WriteLine("로그인 상태 업데이트 실패");
					}
					else
					{
						Debug.WriteLine("로그인 상태 업데이트 완료");
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		/// <summary>
		/// 로그아웃 가능한지 확인
		/// </summary>
		/// <param name="id">대상 아이디</param>
		public bool IsLogoutPossible(string id)
		{
			try
			{
				DataSet dataSet = new DataSet();

				using (MySqlConnection mySql = new MySqlConnection(_connectionAddress))
				{
					mySql.Open();

					//유효한 아이디이며 해당 아이디가 로그인 상태일 때 
					string query = "SELECT * FROM user_tb WHERE id = @id AND id = (SELECT id FROM connection_tb WHERE login_state = true)";

					MySqlCommand cmd = new MySqlCommand(query, mySql);

					MySqlParameter idParam = new MySqlParameter("@id", MySqlDbType.VarChar);
					idParam.Value = id;
					cmd.Parameters.Add(idParam);

					MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
					adapter.Fill(dataSet);

					if (dataSet.Tables[0].Rows.Count > 0)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);

				return false;
			}
		}

        /// <summary>
        /// 대화방 목록 가져오기
        /// </summary>
        /// <param name="id">아이디</param>
        public string[] GetChatRoomList(string id)
        {
			List<string> chatRoomList = new List<string>();

            List<string> roomIdList = new List<string>();

            try
            {
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();

                    string query = "SELECT room_id FROM chatroom_member_tb WHERE user_id = @id";

                    MySqlCommand cmd = new MySqlCommand(query, mysql);

                    MySqlParameter idParam = new MySqlParameter("@id", MySqlDbType.VarChar);
                    idParam.Value = id;
                    cmd.Parameters.Add(idParam);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //방 번호 가져오기
                            roomIdList.Add(reader["room_id"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

			StringBuilder stringBuilder = new StringBuilder();

			try
            {
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();

                    string query = "SELECT user_id FROM chatroom_member_tb WHERE room_id = @roomId";

                    foreach (string roomId in roomIdList)
                    {
						//방 번호 먼저 추가
						stringBuilder.Append(roomId);

						MySqlCommand cmd = new MySqlCommand(query, mysql);

                        MySqlParameter roomIdParam = new MySqlParameter("@roomId", MySqlDbType.Int64);
                        roomIdParam.Value = int.Parse(roomId);
                        cmd.Parameters.Add(roomIdParam);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //방 맴버들 이어 추가
                                stringBuilder.Append("," + reader["user_id"].ToString());
                            }
                        }

						chatRoomList.Add(stringBuilder.ToString());
						stringBuilder.Clear();
                    }
				}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

			return chatRoomList.ToArray();
        }

        /// <summary>
        /// 친구 목록을 가져온다.
        /// 친구 목록은 아이디, 로그인 상태로 구성된다.
        /// </summary>
        /// <returns></returns>
        public string[] GetFriendList()
        {
			//아이디 목록과 로그인 상태 목록을 함께 담는다
			List<string> friendList = new List<string>();

			try
			{
				DataSet dataSet = new DataSet();

				using (MySqlConnection mySql = new MySqlConnection(_connectionAddress))
				{
					mySql.Open();

					//아이디, 비밀번호가 DB의 값과 일치하면 로그인
					string query = "SELECT * FROM connection_tb";

					MySqlCommand cmd = new MySqlCommand(query, mySql);

					MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
					adapter.Fill(dataSet);

					int count = dataSet.Tables[0].Rows.Count;

					string friends = "";

					for (int i = 0; i < count; i++)
                    {
						//아이디 가져오기
						friends += dataSet.Tables[0].Rows[i]["user_id"].ToString() + ",";
					}

					//아이디 목록 먼저 추가
					friendList.Add(friends.Substring(0, friends.Length - 1));

					string loginState = "";

					for (int i = 0; i < count; i++)
					{
						//해당 아이디의 로그인 상태 가져오기
						loginState += dataSet.Tables[0].Rows[i]["login_state"].ToString() + ",";
					}

					//로그인 상태 목록 이어 추가
					friendList.Add(loginState.Substring(0, loginState.Length - 1));
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return friendList.ToArray();
		}

		/// <summary>
		/// 새로운 대화방 생성을 위한 대화방 번호 생성(AUTO INCREMENTAL)
		/// </summary>
		public void GenerateRoomNumber()
		{
			try
			{
				using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
				{
					mysql.Open();

					string query = "INSERT INTO chatroom_tb () VALUES ();";

					MySqlCommand cmd = new MySqlCommand(query, mysql);

					if (cmd.ExecuteNonQuery() != 1)
					{
						Debug.WriteLine("대화방 생성 실패");
					}
                    else
                    {
						Debug.WriteLine("대화방 생성 완료");
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		/// <summary>
		/// 가장 최근에 생성된 대화방의 번호를 반환한다
		/// </summary>
		public int GetRoomNumberLastCreated()
		{
			int roomNumber = 0;

			try
			{
				DataSet dataSet = new DataSet();	

				using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
				{
					mysql.Open();

					string query = "SELECT room_id AS last_room_id FROM chatroom_tb WHERE room_id = @@Identity;";

					MySqlCommand cmd = new MySqlCommand(query, mysql);

					MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
					adapter.Fill(dataSet);

					if (dataSet.Tables[0].Rows.Count > 0)
					{
						roomNumber = int.Parse(dataSet.Tables[0].Rows[0]["last_room_id"].ToString());
					}
					else
					{
						Debug.WriteLine("최신 생성 대화방 번호 가져오기 실패");
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return roomNumber;
		}

		/// <summary>
		/// 매개변수로 받은 대화방 맴버로 새로운 대화방을 생성하고 해당 대화방의 번호를 반환한다
		/// </summary>
		/// <param name="roomMember">대화방 구성 맴버</param>
		public int CreateChatRoom(string[] roomMember)
        {
			int roomNumber = 0;

			try
            {
				//방 번호 생성
				GenerateRoomNumber();
				//생성한 방 번호 가져오기
				roomNumber = GetRoomNumberLastCreated();

                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
					mysql.Open();

					foreach (string memeber in roomMember)
                    {
						string query = string.Format("INSERT INTO chatroom_member_tb (room_id, user_id) VALUES ('{0}', '{1}');", roomNumber, memeber);

						MySqlCommand cmd = new MySqlCommand(query, mysql);

						if (cmd.ExecuteNonQuery() != 1)
						{
							return 0;
						}
					}
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

			return roomNumber;
        }

        /// <summary>
        /// 매개변수로 받은 대화방 번호가 유효한지 확인 후 해당 대화방 번호 반환
        /// </summary>
        /// <param name="roomNumber">대화방 번호</param>
        public int GetExistChatRoom(int roomNumber)
        {
			int roomNumberChecked = 0;

			try
			{
				DataSet dataSet = new DataSet();

				using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
				{
					mysql.Open();

					string query = "SELECT room_id FROM chatroom_tb WHERE room_id = @roomNumber";

					MySqlCommand cmd = new MySqlCommand(query, mysql);

					MySqlParameter param = new MySqlParameter("@roomNumber", MySqlDbType.Int64);
					param.Value = roomNumber;
					cmd.Parameters.Add(param);

					MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
					adapter.Fill(dataSet);

					if (dataSet.Tables[0].Rows.Count > 0)
					{
						if (roomNumber == int.Parse(dataSet.Tables[0].Rows[0]["room_id"].ToString()))
                        {
							roomNumberChecked = roomNumber;
						}
                        else
                        {
							roomNumberChecked = 0;
						}
					}
					else
					{
						Debug.WriteLine("대화방 번호 유효성 검사 실패");
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return roomNumberChecked;
		}

		/// <summary>
		/// 매개변수로 받은 아이디 및 방 번호와 일치하는 대화방의 이전 대화 파일이 있는지 확인
		/// </summary>
		/// <param name="senderId">요쳥자 아이디</param>
		/// <param name="roomNumber">방 번호</param>
		public string GetPrevMsgsFileName(string senderId, int roomNumber)
        {
			string fileName = "";

			try
			{
				DataSet dataSet = new DataSet();

				using (MySqlConnection mySql = new MySqlConnection(_connectionAddress))
				{
					mySql.Open();

					string query = "SELECT file_name FROM chatroom_file_tb WHERE user_id = @senderId AND room_id = @roomNumber";

					MySqlCommand cmd = new MySqlCommand(query, mySql);

					MySqlParameter idParam = new MySqlParameter("@senderId", MySqlDbType.VarChar);
					idParam.Value = senderId;
					cmd.Parameters.Add(idParam);

					MySqlParameter roomNumParam = new MySqlParameter("@roomNumber", MySqlDbType.Int64);
					roomNumParam.Value = roomNumber;
					cmd.Parameters.Add(roomNumParam);

					MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
					adapter.Fill(dataSet);

					if (dataSet.Tables[0].Rows.Count > 0)
					{
						fileName += dataSet.Tables[0].Rows[0]["file_name"];
					}
                    else
                    {
						fileName = null;
                    }
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return fileName;
		}

		/// <summary>
		/// 파일
		/// </summary>
		public void InsertFileInfo(string id, int roomNumber, string fileName, string filePath)
        {
			try
			{
				using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
				{
					mysql.Open();

					string query
						= string.Format("INSERT INTO chatroom_file_tb (room_id, user_id, file_name, file_path) VALUES ('{0}', '{1}', '{2}', '{3}');", roomNumber, id, fileName, filePath);

					MySqlCommand cmd = new MySqlCommand(query, mysql);

					if (cmd.ExecuteNonQuery() != 1)
					{
						Debug.WriteLine("파일 정보 저장 실패");
					}
					else
					{
						Debug.WriteLine("파일 정보 저장 완료");
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		/// <summary>
		/// 대화방 맴버의 Uid를 가져온다
		/// </summary>
		/// <param name="roomNumber">대화방 번호</param>
		public int[] GetChatRoomMemberUid(int roomNumber)
        {
			List<int> uidList = new List<int>();

			try
			{
				DataSet dataSet = new DataSet();

				using (MySqlConnection mySql = new MySqlConnection(_connectionAddress))
				{
					mySql.Open();

					string query = "SELECT uid FROM chatroom_member_tb c, user_tb u WHERE c.room_id = @roomNumber AND c.user_id = u.id";

					MySqlCommand cmd = new MySqlCommand(query, mySql);

					MySqlParameter param = new MySqlParameter("@roomNumber", MySqlDbType.Int64);
					param.Value = roomNumber;
					cmd.Parameters.Add(param);

					MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
					adapter.Fill(dataSet);

					int count = dataSet.Tables[0].Rows.Count;

					for(int i = 0; i < count; i++)
                    {
						uidList.Add(int.Parse(dataSet.Tables[0].Rows[i]["uid"].ToString()));
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return uidList.ToArray();
		}


		/// <summary>
		/// 대화방에 맴버를 추가한다
		/// </summary>
		/// <param name="roomNumber">방 번호</param>
		/// <param name="roomMember">초대 맴버</param>
		public bool AddFriendsToChatRoom(int roomNumber, string[] roomMember)
		{
			try
			{
				using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
				{
					mysql.Open();

					foreach (string memeber in roomMember)
					{
						string query = string.Format("INSERT INTO chatroom_member_tb (room_id, user_id) VALUES ('{0}', '{1}');", roomNumber, memeber);

						MySqlCommand cmd = new MySqlCommand(query, mysql);

						if (cmd.ExecuteNonQuery() != 1)
						{
							return false;
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return true;
		}


		/// <summary>
		/// 초대 가능 친구 목록을 가져온다.
		/// 친구 목록은 아이디, 로그인 상태로 구성된다.
		/// </summary>
		/// <returns></returns>
		public List<string> GetInvitationList(int roomNumber)
		{
			//아이디 목록과 로그인 상태 목록을 함께 담는다
			List<string> invitationList = new List<string>();

			try
			{
				DataSet dataSet = new DataSet();

				using (MySqlConnection mySql = new MySqlConnection(_connectionAddress))
				{
					mySql.Open();

					string query = "SELECT * FROM connection_tb c WHERE c.user_id NOT IN (SELECT user_id FROM chatroom_member_tb WHERE room_id = @roomNumber)";

					MySqlCommand cmd = new MySqlCommand(query, mySql);

					MySqlParameter param = new MySqlParameter("@roomNumber", MySqlDbType.Int64);
					param.Value = roomNumber;
					cmd.Parameters.Add(param);

					MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
					adapter.Fill(dataSet);

					int count = dataSet.Tables[0].Rows.Count;

					for (int i = 0; i < count; i++)
					{
						invitationList.Add(dataSet.Tables[0].Rows[i]["user_id"].ToString());
					}

					for (int i = 0; i < count; i++)
					{
						invitationList.Add(dataSet.Tables[0].Rows[i]["login_state"].ToString());
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return invitationList;
		}

		/// <summary>
		/// 대화방 맴버를 가져온다
		/// </summary>
		/// <param name="roomNumber">대화방 번호</param>
		public string[] GetChatRoomMember(int roomNumber)
		{
			List<string> memberList = new List<string>();

			try
			{
				DataSet dataSet = new DataSet();

				using (MySqlConnection mySql = new MySqlConnection(_connectionAddress))
				{
					mySql.Open();

					string query = "SELECT * FROM connection_tb c WHERE c.user_id IN (SELECT user_id FROM chatroom_member_tb WHERE room_id = @roomNumber) ";

					MySqlCommand cmd = new MySqlCommand(query, mySql);

					MySqlParameter param = new MySqlParameter("@roomNumber", MySqlDbType.Int64);
					param.Value = roomNumber;
					cmd.Parameters.Add(param);

					MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
					adapter.Fill(dataSet);

					int count = dataSet.Tables[0].Rows.Count;

					for (int i = 0; i < count; i++)
					{
						memberList.Add(dataSet.Tables[0].Rows[i]["user_id"].ToString());
						//memberList.Add(int.Parse(dataSet.Tables[0].Rows[i]["login_state"].ToString()));
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return memberList.ToArray();
		}

		/// <summary>
		/// 대화방에서 해당 맴버를 제거한다
		/// </summary>
		/// <param name="roomNumber">방 번호</param>
		/// <param name="id">맴버 아이디</param>
		public bool RemoveUserFromChatRoom(int roomNumber, string id)
        {
			try
			{
				DataSet dataSet = new DataSet();

				using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
				{
					mysql.Open();

					string query = "DELETE FROM chatroom_member_tb WHERE room_id = @roomNumber AND user_id = @id";

					MySqlCommand cmd = new MySqlCommand(query, mysql);

					MySqlParameter idParam = new MySqlParameter("@id", MySqlDbType.VarChar);
					idParam.Value = id;
					cmd.Parameters.Add(idParam);

					MySqlParameter roomNumParam = new MySqlParameter("@roomNumber", MySqlDbType.Int64);
					roomNumParam.Value = roomNumber;
					cmd.Parameters.Add(roomNumParam);

					if (cmd.ExecuteNonQuery() != 1)
					{
						Debug.WriteLine("방 나가기 실패");
						return false;
					}
					else
					{
						Debug.WriteLine("방 나가기 성공");
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return true;
		}
	}
}
